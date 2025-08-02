using ECommerce.Common.Clients;
using OrderService.Models.v1.DTOs.External;

namespace OrderService.ExternalServices
{
    public interface IExternalPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(Guid userId, PaymentRequest paymentRequest);
    }
}
