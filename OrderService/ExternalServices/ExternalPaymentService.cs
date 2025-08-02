using ECommerce.Common.Clients;
using ECommerce.Common.Helpers;
using OrderService.Configurations;
using OrderService.Models.v1.DTOs.External;
using System.Text;

namespace OrderService.ExternalServices
{
    public class ExternalPaymentService : BaseExternalService, IExternalPaymentService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private static string API_PROCESS_PAYMENT(Guid userId) => $"api/internal/v1/users/{userId}/payments";

        public ExternalPaymentService(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.PaymentService;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(Guid userId, PaymentRequest paymentRequest)
        {
            var uri = $"{_baseServiceUrl}/{API_PROCESS_PAYMENT(userId)}";

            var contentString = Serializer.SerializeObject(paymentRequest, ignoreNull: true);

            var content = new StringContent(contentString, Encoding.UTF8, JSON_CONTENT);

            var response = await _httpClientWrapper.PostAsync<PaymentResponse>(
                uri: uri,
                requestContent: content,
                errorResponseConversionDelegateAsync: HttpErrorsToServiceExceptionConverter()
                );

            return response;
        }
    }
}
