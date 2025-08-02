using ECommerce.Common.Clients;

namespace NotificationService.ExternalServices
{
    public interface INoRetryHttpClientWrapper
    {
        IHttpClientWrapper GetHttpClientWrapper();
    }
}