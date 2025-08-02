using Microsoft.Extensions.Logging;
using NotificationListener.ExternalServices;
using NotificationListener.Models.DTOs;
using NotificationListener.Models.Enums;

namespace NotificationListener.Services
{
    public class SendNotificationHandlingService : ISendNotificationHandlingService
    {
        private readonly IExternalOrderService _externalOrderService;
        private readonly IExternalNotificationService _externalInvoiceService;
        private readonly ILogger<SendNotificationHandlingService> _logger;

        private static string SUBJECT_PAYMENT_SUCCESS(string orderName) => $"your order is confirmed - {orderName}";
        private static string SUBJECT_PAYMENT_FAILED(string orderName) => $"Your order is holded - {orderName}";
        private static string BODY_PAYMENT_SUCCESS(string orderName) => $"Hello Customer, \nThank you! We’ve successfully received your order ({orderName}) and have started processing it for shipment.";
        private static string BODY_PAYMENT_FAILED(string orderName) => $"Hello Customer, \nUnfortunately, we’re unable to process your order ({orderName}). Verify your payment method or use a different one.";

        public SendNotificationHandlingService(IExternalOrderService externalOrderService, IExternalNotificationService externalInvoiceService, ILogger<SendNotificationHandlingService> logger)
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

                var emailNotificationRequest = new SendNotificationRequest
                {
                    Subject = message.Status == OrderStatus.PaymentSuccess ? SUBJECT_PAYMENT_SUCCESS(orderInfo.Name) : SUBJECT_PAYMENT_FAILED(orderInfo.Name),
                    Body = message.Status == OrderStatus.PaymentSuccess ? BODY_PAYMENT_SUCCESS(orderInfo.Name) : BODY_PAYMENT_FAILED(orderInfo.Name),
                    To = "customer@example.com",
                };

                await _externalInvoiceService.SendEmailNotificationRequestAsync(message.UserId, emailNotificationRequest);

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
