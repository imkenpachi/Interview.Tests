using ECommerce.Common.Clients;
using Microsoft.Extensions.DependencyInjection;
using ProductionListener.Configurations;
using ProductionListener.ExternalServices;
using ProductionListener.Models.DTOs;
using ProductionListener.Models.Enums;
using ProductionListener.Services;
namespace ProductionListener;

public class Worker : BackgroundService
{
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly IProductionRequestHandlingService _requestHandlingService;
    private readonly IExternalOrderService _externalOrderService;

    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, IServiceConfiguration serviceConfiguration, ILogger<Worker> logger)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var clientFactory = scope.ServiceProvider.GetRequiredService<IClientFactory>();
            _messageQueueClient = clientFactory.GetQueueClient("ConnectionStrings:ProductionServiceSender", serviceConfiguration.MessageQueueSettings.RequestProductionQueueName);
            _requestHandlingService = scope.ServiceProvider.GetRequiredService<IProductionRequestHandlingService>();
            _externalOrderService = scope.ServiceProvider.GetRequiredService<IExternalOrderService>();
        }
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
        }
        _messageQueueClient.RegisterMessageHandler<UpdateOrderPaymentStatusMessage>(HandleProductionRequestAsync, default, HandleExceptionWithMessageAsync); //TODO: pass CancellationToken to handler
    }

    private async Task HandleProductionRequestAsync(UpdateOrderPaymentStatusMessage message)
    {
        var (responseCode, note) = await _requestHandlingService.ProcessMessageAsync(message);

        var jobStatus = responseCode == ResponseCode.Success ? ProcessJobStatus.Completed : ProcessJobStatus.Error;

        var request = new UpdateProcessJobStatusRequest
        {
            JobStatus = jobStatus.ToString(),
            JobType = ProcessJobType.Production.ToString(),
            Note = note,
        };

        await _externalOrderService.UpdateProcessJobStatus(message.ProcessId, request);
    }

    private async Task HandleExceptionWithMessageAsync(Exception exception, UpdateOrderPaymentStatusMessage message)
    {
        await _requestHandlingService.ProcessExceptionAsync(exception, message);
    }
}
