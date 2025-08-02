using ECommerce.Common.Clients;

namespace NotificationListener.ExternalServices
{
    public class NoRetryHttpClientWrapper : INoRetryHttpClientWrapper
    {
        public readonly IHttpClientWrapper _httpClientWrapper;

        public NoRetryHttpClientWrapper(HttpClient httpClient)
        {
            _httpClientWrapper = new HttpClientWrapper(httpClient);
        }

        public IHttpClientWrapper GetHttpClientWrapper() => _httpClientWrapper;
    }
}
