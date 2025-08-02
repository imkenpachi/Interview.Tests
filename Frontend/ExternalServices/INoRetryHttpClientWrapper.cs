using ECommerce.Common.Clients;

namespace Frontend.ExternalServices
{
    public interface INoRetryHttpClientWrapper
    {
        IHttpClientWrapper GetHttpClientWrapper();
    }
}