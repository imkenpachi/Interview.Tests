using Amazon.SQS;
using Amazon.SQS.Model;
using ECommerce.Common.Helpers;
using ECommerce.Common.Infrastructures.MessageQueue;
using ECommerce.Common.Infrastructures.Tracers;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Clients.AWS
{
    public class SQSMessageQueueClient : BaseAWSMessageQueueClient, IMessageQueueClient
    {
        private bool _disposed;
        private readonly IAmazonSQS _sqsConnection;
        private readonly string _queueUrl;
        public CancellationToken CancellationToken { get; internal set; }
        public CancellationTokenSource? CancellationTokenSource { get; set; }

        public int ReceiveRequestWaitTimeSeconds { get; set; } = 20;
        public int ReceiveRequestVisibilityTimeoutSeconds { get; set; } = 60;
        public int ReceiveRequestMaxNumberOfMessages { get; set; } = 1;
        public int SendRequestDelaySeconds { get; set; } = 0;
        public bool EnableMessageDeduplicationId { get; set; } = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                return;
            }

            if (disposing)
            {
                CancellationTokenSource?.Cancel();
                CancellationTokenSource?.Dispose();
                CancellationTokenSource = null;
                _sqsConnection?.Dispose();
            }

            _disposed = true;
        }

        internal delegate void EventHandler(CancellationTokenSource cancellationTokenSource);
        internal event EventHandler? OnPollCompleted;

        public SQSMessageQueueClient(IAmazonSQS amazonSQS,
            string queueUrl,
            string queueName,
            string queueGroup,
            IHttpContextAccessor httpContextAccessor,
            ICorrelationIdGenerator correlationIdGenerator,
            ILocalCorrelationIdAccessor localCorrelationIdAccessor
            ) : base(queueName, queueGroup, localCorrelationIdAccessor, httpContextAccessor, correlationIdGenerator)
        {
            _sqsConnection = amazonSQS;
            _queueUrl = queueUrl;
        }

        public async Task SendMessageAsync<T>(T message)
        {
            var correlationModel = CorrelationIdHelpers.GetCorrelation(LocalCorrelationIdAccessor, HttpContextAccessor, CorrelationIdGenerator);

            var data = QueueMessageSerializer.SerializeObject(message, correlationModel);

            Log.Information("Publishing {SizeInBytes} bytes message of type {Type} to queueName {QueueName}, queueGroup {QueueGroup}");

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                //MessageGroupId = QueueGroup,
                MessageBody = data,
                DelaySeconds = SendRequestDelaySeconds,
            };

            if (EnableMessageDeduplicationId)
            {
                sendMessageRequest.MessageDeduplicationId = Guid.NewGuid().ToString("N");
            }

            await _sqsConnection.SendMessageAsync(sendMessageRequest);
        }

        public void RegisterMessageHandler<T>(Func<T, Task> messageHandler, Func<Exception, Task> exceptionHandler, Func<Exception, T, Task> exceptionWithMessageHandler)
        {
            Log.Information("Subscribing for messages of type {Type} in QueueName={QueueName}, QueueGroup={QueueGroup}.", typeof(T), QueueName, QueueGroup);

            CancellationTokenSource = CancellationTokenSource ?? new CancellationTokenSource();
            CancellationToken = CancellationTokenSource.Token;

            Task.Run(async () =>
            {
                while (!CancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var receiveMessageRequest = new ReceiveMessageRequest
                        {
                            QueueUrl = _queueUrl,
                            WaitTimeSeconds = ReceiveRequestWaitTimeSeconds,
                            VisibilityTimeout = ReceiveRequestVisibilityTimeoutSeconds,
                            MaxNumberOfMessages = ReceiveRequestMaxNumberOfMessages,
                            ReceiveRequestAttemptId = Guid.NewGuid().ToString("N")
                        };

                        // VisibilityTimeout value need to be selected properly: an acceptable message processing duration, with
                        // FIFO queue having one queue group, message will never be received until VisibilityTime is timed-out.
                        var receiveMessageResponse = await _sqsConnection.ReceiveMessageAsync(receiveMessageRequest);
                        int count = (receiveMessageResponse.Messages?.Count) ?? 0;

                        Log.Debug("Received {MessageCount} messages, {ReceiveMessageRequest}, {ReceiveMessageResponse}", count, receiveMessageRequest, receiveMessageResponse);

                        if (count == 0)
                        {
                            continue;
                        }

                        foreach (var message in receiveMessageResponse.Messages)
                        {
                            // If there were errors, the processing should continue as there is no use-case for messages to be processed in strict order.
                            await this.ProcessMessageAsync(messageHandler, exceptionHandler, exceptionWithMessageHandler, message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, "Fatal error when processing message {QueueName}, {QueueGroup}", QueueName, QueueGroup);
                    }
                    finally
                    {
                        OnPollCompleted?.Invoke(CancellationTokenSource);
                    }
                }
            });
        }

        private async Task ProcessMessageAsync<T>(Func<T, Task> messageHandler, Func<Exception, Task> exceptionHandler, Func<Exception, T, Task> exceptionWithMessageHandler, Message message)
        {
            var queueMessageSession = new QueueMessageSession<T>(CorrelationIdGenerator.GenerateNewCorrelation())
            {
                CorrelationIdManager = new CorrelationIdManager(CorrelationIdGenerator),
            };

            using (queueMessageSession.CorrelationModel?.EnrichLogContext())
            {
                Log.Information("Received {SizeInBytes} bytes message of type {Type} in QueueName={QueueName}, QueueGroup={QueueGroup}, {ReceiptHandle}",
queueMessageSession.MessageSize, typeof(T), QueueName, QueueGroup, message.ReceiptHandle);

                var stopwatch = new Stopwatch();

                stopwatch.Start();

                try
                {
                    byte[] messageData = Encoding.UTF8.GetBytes(message.Body);
                    queueMessageSession.MessageSize = messageData?.Length ?? 0;
                    queueMessageSession.MessageData = messageData;

                    await HandleMessageAsync(queueMessageSession, messageHandler);

                    // Message should be deleted after successful process.
                    await DeleteMessageAsync(queueMessageSession, message);
                }
                catch (Exception ex)
                {
                    // LeaveMessageForOtherPolls will keep exceptions in logs instead of being thrown and interupting the process,
                    // This is to ensure HandleExceptionAsync be called.
                    await LeaveMessageForOtherPollsAsync(queueMessageSession, message);
                    await HandleExceptionAsync(queueMessageSession, exceptionHandler, exceptionWithMessageHandler, ex);
                }
                finally
                {
                    stopwatch.Stop();
                    // The SessionCorrelation maybe changed, so we should enrich log context with updated Correlation
                    using (queueMessageSession.CorrelationModel?.EnrichLogContext())
                    {
                        Log.Information("Processed {SizeInBytes} bytes message in {ElapsedMilliseconds}ms.", queueMessageSession.MessageSize, stopwatch.ElapsedMilliseconds);
                    }
                }
            }
        }

        private async Task DeleteMessageAsync<T>(QueueMessageSession<T> queueMessageSession, Message message)
        {
            using (queueMessageSession.CorrelationModel?.EnrichLogContext())
            {
                Log.Debug("Deleting message with ReceiptHandle {ReceiptHandle} after processing.", message.ReceiptHandle);
                try
                {
                    await _sqsConnection.DeleteMessageAsync(new DeleteMessageRequest { QueueUrl = _queueUrl, ReceiptHandle = message.ReceiptHandle });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occur when deleting message after it was consumed {ReceiptHandle}", message.ReceiptHandle);
                    throw;
                }
            }
        }

        /// <summary>
        /// This method set message visibility to 0 which will make the message immediately available for being picked-up in other polls.
        /// </summary>
        private async Task LeaveMessageForOtherPollsAsync<T>(QueueMessageSession<T> queueMessageSession, Message message)
        {
            const int messageVisibility = 0;

            using (queueMessageSession.CorrelationModel?.EnrichLogContext())
            {
                try
                {
                    Log.Error("Changing visibility of message with ReceiptHandle {ReceiptHandle} to {MessageVisibility} in {QueueUr1}",
                        message.ReceiptHandle, messageVisibility, _queueUrl);

                    await _sqsConnection.ChangeMessageVisibilityAsync(_queueUrl, message.ReceiptHandle, messageVisibility);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occur when changing visibility of message with {ReceiptHandle} to {MessageVisibility}", message.ReceiptHandle, messageVisibility);
                }
            }
        }
    }
}
