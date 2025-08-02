using ECommerce.Common.Clients;

namespace OrderService.ExternalServices
{
    public interface INoRetryHttpClientWrapper
    {
        IHttpClientWrapper GetHttpClientWrapper();
    }
}