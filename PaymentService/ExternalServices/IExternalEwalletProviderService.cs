using ECommerce.Common.Clients;
using PaymentService.Models.v1.DTOs.External;

namespace PaymentService.ExternalServices
{
    public interface IExternalEwalletProviderService
    {
        Task<EwalletPaymentResponse> CreateEwalletPaymentRequestAsync(EwalletPaymentRequest paymentRequest);
    }
}
