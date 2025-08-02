using ECommerce.Common.Clients;
using ECommerce.Common.Helpers;
using PaymentService.Configurations;
using PaymentService.Models.v1.DTOs.External;
using System.Text;

namespace PaymentService.ExternalServices
{
    public class ExternalEwalletProviderService : BaseExternalService, IExternalEwalletProviderService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private const string API_EWALLET_PAYMENT = "api/payments/payment-requests";

        public ExternalEwalletProviderService(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.EwalletProvider;
        }

        public async Task<EwalletPaymentResponse> CreateEwalletPaymentRequestAsync(EwalletPaymentRequest paymentRequest)
        {
            var uri = $"{_baseServiceUrl}/{API_EWALLET_PAYMENT}";

            var contentString = Serializer.SerializeObject(paymentRequest, ignoreNull: true);

            var content = new StringContent(contentString, Encoding.UTF8, JSON_CONTENT);

            var response = await _httpClientWrapper.PostAsync<EwalletPaymentResponse>(
                uri: uri,
                requestContent: content,
                errorResponseConversionDelegateAsync: HttpErrorsToServiceExceptionConverter()
                );

            return response;
        }
    }
}
