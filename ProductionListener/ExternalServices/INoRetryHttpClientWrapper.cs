using ECommerce.Common.Clients;

namespace ProductionListener.ExternalServices
{
    public interface INoRetryHttpClientWrapper
    {
        IHttpClientWrapper GetHttpClientWrapper();
    }
}