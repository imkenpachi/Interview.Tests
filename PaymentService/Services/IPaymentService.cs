using PaymentService.Models.v1.DTOs;
using PaymentService.Models.v1.Enums;

namespace PaymentService.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(Guid userId, PaymentRequest paymentRequest);
        Task HandleWebHookNotificationAsync(WebhookNotificationMessage message);
    }
}
