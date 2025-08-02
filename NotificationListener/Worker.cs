using ECommerce.Common.Clients;
using Microsoft.Extensions.DependencyInjection;
using NotificationListener.Configurations;
using NotificationListener.ExternalServices;
using NotificationListener.Models.DTOs;
using NotificationListener.Models.Enums;
using NotificationListener.Services;
namespace NotificationListener;

public class Worker : BackgroundService
{
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly ISendNotificationHandlingService _requestHandlingService;
    private readonly IExternalOrderService _externalOrderService;

    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, IServiceConfiguration serviceConfiguration, ILogger<Worker> logger)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var clientFactory = scope.ServiceProvider.GetRequiredService<IClientFactory>();
            _messageQueueClient = clientFactory.GetQueueClient("ConnectionStrings:NotificationServiceSender", serviceConfiguration.MessageQueueSettings.SendNotificationQueueName);
            _requestHandlingService = scope.ServiceProvider.GetRequiredService<ISendNotificationHandlingService>();
            _externalOrderService = scope.ServiceProvider.GetRequiredService<IExternalOrderService>();
        }
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
        _messageQueueClient.RegisterMessageHandler<UpdateOrderPaymentStatusMessage>(HandleEmailNotificationAsync, default, HandleExceptionWithMessageAsync); //TODO: pass CancellationToken to handler
        _logger.LogInformation("Worker stopped at: {Time}", DateTimeOffset.Now);
	}

	private async Task HandleEmailNotificationAsync(UpdateOrderPaymentStatusMessage message)
    {
        var (responseCode, note) = await _requestHandlingService.ProcessMessageAsync(message);

        var jobStatus = responseCode == ResponseCode.Success ? ProcessJobStatus.Completed : ProcessJobStatus.Error;

        var request = new UpdateProcessJobStatusRequest
        {
            JobStatus = jobStatus.ToString(),
            JobType = ProcessJobType.Notification.ToString(),
            Note = note,
        };

        await _externalOrderService.UpdateProcessJobStatus(message.ProcessId, request);
    }

    private async Task HandleExceptionWithMessageAsync(Exception exception, UpdateOrderPaymentStatusMessage message)
    {
        await _requestHandlingService.ProcessExceptionAsync(exception, message);
    }
}
