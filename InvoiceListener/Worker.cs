using ECommerce.Common.Clients;
using InvoiceListener.Configurations;
using InvoiceListener.ExternalServices;
using InvoiceListener.Models.DTOs;
using InvoiceListener.Models.Enums;
using InvoiceListener.Services;
namespace InvoiceListener;

public class Worker : BackgroundService
{
    private readonly IMessageQueueClient _messageQueueClient;
    private readonly IInvoiceGenerationHandlingService _requestHandlingService;
    private readonly IExternalOrderService _externalOrderService;

    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, IServiceConfiguration serviceConfiguration, ILogger<Worker> logger)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var clientFactory = scope.ServiceProvider.GetRequiredService<IClientFactory>();
            _messageQueueClient = clientFactory.GetQueueClient("ConnectionStrings:InvoiceServiceSender", serviceConfiguration.MessageQueueSettings.GenerateInvoiceQueueName);
            _requestHandlingService = scope.ServiceProvider.GetRequiredService<IInvoiceGenerationHandlingService>();
            _externalOrderService = scope.ServiceProvider.GetRequiredService<IExternalOrderService>();
        }
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
        _messageQueueClient.RegisterMessageHandler<UpdateOrderPaymentStatusMessage>(HandleInvoiceGenerationAsync, default, HandleExceptionWithMessageAsync); //TODO: pass CancellationToken to handler
        _logger.LogInformation("Worker stopped at: {Time}", DateTimeOffset.Now);
	}

	private async Task HandleInvoiceGenerationAsync(UpdateOrderPaymentStatusMessage message)
    {
        var (responseCode, note) = await _requestHandlingService.ProcessMessageAsync(message);

        var jobStatus = responseCode == ResponseCode.Success ? ProcessJobStatus.Completed : ProcessJobStatus.Error;

        var request = new UpdateProcessJobStatusRequest
        {
            JobStatus = jobStatus.ToString(),
            JobType = ProcessJobType.Invoice.ToString(),
            Note = note,
        };

        await _externalOrderService.UpdateProcessJobStatus(message.ProcessId, request);
    }

    private async Task HandleExceptionWithMessageAsync(Exception exception, UpdateOrderPaymentStatusMessage message)
    {
        await _requestHandlingService.ProcessExceptionAsync(exception, message);
    }
}
