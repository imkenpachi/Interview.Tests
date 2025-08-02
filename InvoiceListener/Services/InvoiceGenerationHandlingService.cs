using Microsoft.Extensions.Logging;
using InvoiceListener.ExternalServices;
using InvoiceListener.Models.DTOs;
using InvoiceListener.Models.Enums;

namespace InvoiceListener.Services
{
    public class InvoiceGenerationHandlingService : IInvoiceGenerationHandlingService
    {
        private readonly IExternalOrderService _externalOrderService;
        private readonly IExternalInvoiceService _externalInvoiceService;
        private readonly ILogger<InvoiceGenerationHandlingService> _logger;

        public InvoiceGenerationHandlingService(IExternalOrderService externalOrderService, IExternalInvoiceService externalInvoiceService, ILogger<InvoiceGenerationHandlingService> logger)
        {
            _externalOrderService = externalOrderService;
            _externalInvoiceService = externalInvoiceService;
            _logger = logger;
        }

        public Task ProcessExceptionAsync(Exception exception, UpdateOrderPaymentStatusMessage message)
        {
            _logger.LogInformation("Processing production request exception for order {OrderId}, process {ProcessId}, for user {UserId}", message.OrderId, message.ProcessId, message.UserId);

            return Task.FromResult(0);
        }

        public async Task<(ResponseCode, string note)> ProcessMessageAsync(UpdateOrderPaymentStatusMessage message)
        {
            _logger.LogInformation("Processing invoice generation request for order {OrderId}, process {ProcessId}, for user {UserId}", message.OrderId, message.ProcessId, message.UserId);

            try
            {
                var orderInfo = await _externalOrderService.GetOrderInfoAsync(message.UserId, message.OrderId);

                var invoiceGenerationRequest = new InvoiceGenerationRequest
                {
                    OrderId = orderInfo.Id,
                    OrderName = orderInfo.Name,
                    UserId = message.UserId,
                    OrderStatus = orderInfo.Status.ToString(),
                    OrderDetails = orderInfo.OrderDetails,
                };

                await _externalInvoiceService.SendInvoiceGenerationRequestAsync(message.UserId, invoiceGenerationRequest);

                return (ResponseCode.Success, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while processing production request for order {OrderId}, process {ProcessId}, for user {UserId}", message.OrderId, message.ProcessId, message.UserId);
                return (ResponseCode.ExternalServiceCallFailed, ex.Message);
            }
        }
    }
}
