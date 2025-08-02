using Amazon.Runtime.Internal;
using Azure.Core;
using ECommerce.Common.Clients;
using OrderService.Configurations;
using OrderService.Models.v1.DomainModels;
using OrderService.Models.v1.DTOs;
using OrderService.Models.v1.Enums;
using OrderService.Repositories;

namespace OrderService.Services
{
    public class OrderProcessService : IOrderProcessService
    {
        private readonly IOrderProcessRepository _orderProcessRepository;
        private readonly IOrderProcessJobRepository _orderProcessJobRepository;
        private readonly ILogger<OrderProcessService> _logger;
        private readonly IMessageQueueClient _productionQueueClient;
        private readonly IMessageQueueClient _invoiceQueueClient;
        private readonly IMessageQueueClient _notificationQueueClient;
        public OrderProcessService(IOrderProcessRepository orderProcessRepository, IOrderProcessJobRepository orderProcessJobRepository, ILogger<OrderProcessService> logger, IClientFactory clientFactory, IServiceConfiguration serviceConfiguration)
        {
            _orderProcessRepository = orderProcessRepository;
            _orderProcessJobRepository = orderProcessJobRepository;
            _logger = logger;

            _productionQueueClient = clientFactory.GetQueueClient("ConnectionStrings:ProductionServiceSender", serviceConfiguration.MessageQueueSettings.RequestProductionQueueName);
            _invoiceQueueClient = clientFactory.GetQueueClient("ConnectionStrings:InvoiceServiceSender", serviceConfiguration.MessageQueueSettings.GenerateInvoiceQueueName);
            _notificationQueueClient = clientFactory.GetQueueClient("ConnectionStrings:NotificationServiceSender", serviceConfiguration.MessageQueueSettings.SendNotificationQueueName);
        }

        public async Task<OrderProcess> CreateOrderProcessAsync(Guid userId, Guid orderId, OrderPaymentProcessFlow orderPaymentProcessFlow)
        {
            var orderProcessJob = orderPaymentProcessFlow == OrderPaymentProcessFlow.Success ? new List<OrderProcessJob> {
                    new() {
                        Id = Guid.NewGuid(),
                        ProcessJobStatus = ProcessJobStatus.Queued,
                        ProcessJobType = ProcessJobType.Production,
                        CreatedBy = userId,
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        ProcessJobStatus = ProcessJobStatus.Queued,
                        ProcessJobType = ProcessJobType.Invoice,
                        CreatedBy = userId,
                    },
                    new() {
                        Id = Guid.NewGuid(),
                        ProcessJobStatus = ProcessJobStatus.Queued,
                        ProcessJobType = ProcessJobType.Notification,
                        CreatedBy = userId,
                    }
                } :
                new List<OrderProcessJob> {
                    new() {
                        Id = Guid.NewGuid(),
                        ProcessJobStatus = ProcessJobStatus.Queued,
                        ProcessJobType = ProcessJobType.Notification,
                        CreatedBy = userId,
                    }
                };

            var orderProcess = new OrderProcess
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Status = ProcessStatus.Pending,
                ProcessJobs = orderProcessJob,
                CreatedBy = userId,
            };

            await _orderProcessRepository.AddAsync(orderProcess);

            var message = new UpdateOrderPaymentStatusMessage
            {
                OrderId = orderId,
                ProcessId = orderProcess.Id,
                Status = orderPaymentProcessFlow == OrderPaymentProcessFlow.Success ? OrderStatus.PaymentSuccess.ToString() : OrderStatus.PaymentFailed.ToString(),
                UserId = userId,
            };

            var publishMessageToQueueTasks = orderProcessJob.Select(async job =>
            {
                IMessageQueueClient messageQueueClient = job.ProcessJobType switch
                {
                    ProcessJobType.Production => _productionQueueClient,
                    ProcessJobType.Invoice => _invoiceQueueClient,
                    ProcessJobType.Notification => _notificationQueueClient,
                    _ => throw new NotImplementedException(),
                };
                await PublishMessageToQueueAsync(messageQueueClient, job.ProcessJobType, message);
            });

            await Task.WhenAll(publishMessageToQueueTasks);

            return orderProcess;
        }

        public async Task UpdateProcessJobStatusAsync(Guid processId, UpdateProcessJobRequest request)
        {
            await UpdateProcessJobStatusAsync(processId, async process =>
            {
                var processJob = process.ProcessJobs.FirstOrDefault(x => x.ProcessJobType == request.JobType);

                if (processJob == null)
                {
                    _logger.LogError("Could not find order process job for {ProcessId}, job type {JobType}", processId, request.JobType);
                    return;
                }

                processJob.ProcessJobStatus = request.JobStatus;
                _orderProcessJobRepository.Update(processJob);

                if (process.ProcessJobs.All(x => x.ProcessJobStatus == ProcessJobStatus.Completed))
                {
                    process.Status = ProcessStatus.Completed;
                }
            });
        }


        private async Task PublishMessageToQueueAsync(
            IMessageQueueClient queueClient,
            ProcessJobType jobType,
            UpdateOrderPaymentStatusMessage message
            )
        {
            try
            {
                await queueClient.SendMessageAsync(message);
                _logger.LogInformation("Successfully published message for order {OrderId}, process {ProcessId}, job {JobType}", message.OrderId, message.ProcessId, jobType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message for order {OrderId}, process {ProcessId}, job {JobType}", message.OrderId, message.ProcessId, jobType);
                var errorNote = $"Failed to publish message for process {message.ProcessId}";
                await HandlePublishMessageToQueueErrorAsync(message.ProcessId, jobType, errorNote);
            }
        }

        private async Task HandlePublishMessageToQueueErrorAsync(Guid processId, ProcessJobType jobType, string errorNote)
        {
            await UpdateProcessJobStatusAsync(processId, process =>
            {
                var processJob = process.ProcessJobs.FirstOrDefault(x => x.ProcessJobType == jobType);

                if (processJob == null)
                {
                    _logger.LogError("Could not find order process job for {ProcessId}, job type {JobType}", processId, jobType);
                    return;
                }

                processJob.ProcessJobStatus = ProcessJobStatus.Error;
                if (!string.IsNullOrEmpty(errorNote))
                {
                    processJob.Note = (processJob.Note ?? string.Empty) + errorNote;
                }
                _orderProcessJobRepository.Update(processJob);

                process.Status = ProcessStatus.Error;
                if (!string.IsNullOrEmpty(errorNote))
                {
                    process.Note = (process.Note ?? string.Empty) + errorNote;
                }
            });
        }

        private async Task<OrderProcess> UpdateProcessJobStatusAsync(Guid processId, Action<OrderProcess> ActionUpdateOrderProcess)
        {
            var processJob = await _orderProcessRepository.GetAsync(processId);
            if (processJob == null)
            {
                _logger.LogError("Could not find order process for {ProcessId}", processId);
                throw new InvalidOperationException();
            }
            ActionUpdateOrderProcess(processJob);
            await _orderProcessRepository.UpdateAsync(processJob);
            return processJob;
        }
    }
}
