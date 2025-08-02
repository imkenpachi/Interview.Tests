using PaymentService.Models.v1.DTOs.External;

namespace PaymentService.ExternalServices
{
    public interface IExternalOrderService
    {
        Task SendPaymentConfirmationAsync(Guid userId, Guid orderId, ConfirmPaymentRequest confirmPaymentRequest);
    }
}
