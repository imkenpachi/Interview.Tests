using ECommerce.Common.Clients;

namespace PaymentService.ExternalServices
{
    public interface INoRetryHttpClientWrapper
    {
        IHttpClientWrapper GetHttpClientWrapper();
    }
}