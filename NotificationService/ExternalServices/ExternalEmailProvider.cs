using ECommerce.Common.Clients;
using ECommerce.Common.Helpers;
using NotificationService.Configurations;
using NotificationService.Models.v1.DTOs.External;
using System.Text;

namespace NotificationService.ExternalServices
{
    public class ExternalEmailProvider : BaseExternalService, IExternalEmailProvider
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private const string API_SEND_EMAIL_NOTIFICATION = "api/v1/transactional-emails";

        public ExternalEmailProvider(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.EmailProvider;
        }

        public async Task<SendEmailResponse> SendEmailNotificationAsync(SendEmailRequest sendEmailRequest)
        {
            var uri = $"{_baseServiceUrl}/{API_SEND_EMAIL_NOTIFICATION}";

            var contentString = Serializer.SerializeObject(sendEmailRequest, ignoreNull: true);

            var content = new StringContent(contentString, Encoding.UTF8, JSON_CONTENT);

            var response = await _httpClientWrapper.PostAsync<SendEmailResponse>(
                uri: uri,
                requestContent: content,
                errorResponseConversionDelegateAsync: HttpErrorsToServiceExceptionConverter()
                );

            return response;
        }
    }
}
