using ECommerce.Common.Clients;
using ECommerce.Common.Helpers;
using ECommerce.Common.Exceptions;
using PaymentService.Configurations;
using PaymentService.Models.v1.DTOs.External;
using System.Text;

namespace PaymentService.ExternalServices
{
    public class ExternalOrderService : BaseExternalService, IExternalOrderService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private readonly ILogger<ExternalOrderService> _logger;
        private static string API_CONFIRM_PAYMENT(Guid userId, Guid orderId) => $"api/internal/v1/users/{userId}/orders/{orderId}/confirm-payment";

        public ExternalOrderService(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration, ILogger<ExternalOrderService> logger)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.OrderService;
            _logger = logger;
        }

        public async Task SendPaymentConfirmationAsync(Guid userId, Guid orderId, ConfirmPaymentRequest confirmPaymentRequest)
        {
            var uri = $"{_baseServiceUrl}/{API_CONFIRM_PAYMENT(userId, orderId)}";

            var contentString = Serializer.SerializeObject(confirmPaymentRequest, ignoreNull: true);

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
