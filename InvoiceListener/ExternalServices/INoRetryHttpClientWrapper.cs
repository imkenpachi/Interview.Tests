using ECommerce.Common.Clients;

namespace InvoiceListener.ExternalServices
{
    public interface INoRetryHttpClientWrapper
    {
        IHttpClientWrapper GetHttpClientWrapper();
    }
}