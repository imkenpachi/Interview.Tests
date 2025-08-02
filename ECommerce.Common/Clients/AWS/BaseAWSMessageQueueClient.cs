using ECommerce.Common.Helpers;
using ECommerce.Common.Infrastructures.MessageQueue;
using ECommerce.Common.Infrastructures.Tracers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Common.Clients.AWS
{
    public abstract class BaseAWSMessageQueueClient
    {
        protected readonly string QueueName;
        protected readonly string QueueGroup;

        protected readonly ILocalCorrelationIdAccessor LocalCorrelationIdAccessor;
        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly ICorrelationIdGenerator CorrelationIdGenerator;

        protected BaseAWSMessageQueueClient(string queueName
            , string queueGroup
            , ILocalCorrelationIdAccessor localCorrelationIdAccessor
            , IHttpContextAccessor httpContextAccessor
            , ICorrelationIdGenerator correlationIdGenerator)
        {
            QueueName = queueName;
            QueueGroup = queueGroup;
            LocalCorrelationIdAccessor = localCorrelationIdAccessor;
            HttpContextAccessor = httpContextAccessor;
            CorrelationIdGenerator = correlationIdGenerator;
        }

        protected async Task HandleMessageAsync<T>(QueueMessageSession<T> queueMessageSession, Func<T?, Task> messageHandler)
        {
            Log.Information("Processed message {@Message}", queueMessageSession);

            if (queueMessageSession.MessageData == null || queueMessageSession.QueueMessage == null)
            {
                Log.Information("Stop handling empty message data");
                return;
            }

            var messageSize = queueMessageSession.MessageData.LongLength;

            if (queueMessageSession.QueueMessage.CorrelationModel != null)
            {
                queueMessageSession.CorrelationModel = queueMessageSession.CorrelationIdManager?.GetNestedCorrelation(queueMessageSession.QueueMessage.CorrelationModel);
            }
            // The SessionCorrelation maybe changed, so we should enrich log context with updated Correlation
            using (queueMessageSession.CorrelationModel?.EnrichLogContext())
            {
                Log.Information("Converted {SizeInBytes} bytes message of type {Type} in QueueName={QueueName}, QueueGroup={QueueGroup}.",
                messageSize, typeof(T), QueueName, QueueGroup);

                LocalCorrelationIdAccessor.Correlation = queueMessageSession.CorrelationModel;

                await messageHandler(queueMessageSession.QueueMessage.Data);
            }
        }

        protected async Task HandleExceptionAsync<T>(
            QueueMessageSession<T?> queueMessageSession,
            Func<Exception, Task> exceptionHandler,
            Func<Exception, T?, Task> exceptionWithMessageHandler,
            Exception exception)
        {
            // The SessionCorrelation maybe changed, so we should enrich log context with updated Correlation
            using (queueMessageSession.CorrelationModel?.EnrichLogContext())
            {
                Log.Error(exception, "Exception thrown when handling message of type {Type} in QueueName={QueueName}, QueueGroup={QueueGroup}.", typeof(T), QueueName, QueueGroup);

                LocalCorrelationIdAccessor.Correlation = queueMessageSession.CorrelationModel;

                if (exceptionHandler != default)
                {
                    await exceptionHandler(exception);
                }

                if (exceptionWithMessageHandler != default)
                {
                    var data = queueMessageSession.QueueMessage is null ? default : queueMessageSession.QueueMessage.Data;

                    await exceptionWithMessageHandler(exception, data);
                }
            }
        }
    }
}
