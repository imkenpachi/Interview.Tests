using ECommerce.Common.Clients;
using ECommerce.Common.Exceptions;
using ECommerce.Common.Helpers;
using Microsoft.Extensions.Logging;
using NotificationListener.Configurations;
using NotificationListener.Models.DTOs;
using System.Text;

namespace NotificationListener.ExternalServices
{
    public class ExternalNotificationService : BaseExternalService, IExternalNotificationService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private readonly ILogger<ExternalNotificationService> _logger;
        private static string API_SEND_EMAIL_NOTIFICATION_REQUEST(Guid userId) => $"api/internal/v1/users/{userId}/emails/send";

        public ExternalNotificationService(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration, ILogger<ExternalNotificationService> logger)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.NotificationService;
            _logger = logger;
        }

        public async Task SendEmailNotificationRequestAsync(Guid userId, SendNotificationRequest emailRequest)
        {
            var uri = $"{_baseServiceUrl}/{API_SEND_EMAIL_NOTIFICATION_REQUEST(userId)}";

            var contentString = Serializer.SerializeObject(emailRequest, ignoreNull: true);

            var content = new StringContent(contentString, Encoding.UTF8, JSON_CONTENT);

            var response = await _httpClientWrapper.PostAsync(
                uri: uri,
                requestContent: content
            );

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to send request to {Uri} with response status code {StatusCode}", uri, response.StatusCode);
                throw new ExternalServiceRequestFailedException();
            }
        }
    }
}
