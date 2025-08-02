using ECommerce.Common.Clients;
using ECommerce.Common.Exceptions;
using ECommerce.Common.Helpers;
using Microsoft.Extensions.Logging;
using InvoiceListener.Configurations;
using InvoiceListener.Models.DTOs;
using System.Text;

namespace InvoiceListener.ExternalServices
{
    public class ExternalInvoiceService : BaseExternalService, IExternalInvoiceService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private readonly ILogger<ExternalInvoiceService> _logger;
        private static string API_INVOICE_GENERATION_REQUEST(Guid userId) => $"api/internal/v1/invoices";

        public ExternalInvoiceService(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration, ILogger<ExternalInvoiceService> logger)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.InvoiceService;
            _logger = logger;
        }

        public async Task SendInvoiceGenerationRequestAsync(Guid userId, InvoiceGenerationRequest productionRequest)
        {
            var uri = $"{_baseServiceUrl}/{API_INVOICE_GENERATION_REQUEST(userId)}";

            var contentString = Serializer.SerializeObject(productionRequest, ignoreNull: true);

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
