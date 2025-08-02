using ECommerce.Common.Clients;
using ECommerce.Common.Exceptions;
using ECommerce.Common.Helpers;
using Microsoft.Extensions.Logging;
using ProductionListener.Configurations;
using ProductionListener.Models.DTOs;
using System.Text;

namespace ProductionListener.ExternalServices
{
    public class ExternalProductionService : BaseExternalService, IExternalProductionService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private readonly ILogger<ExternalProductionService> _logger;
        private static string API_PRODUCTION_REQUEST(Guid userId) => $"api/internal/v1/productions";

        public ExternalProductionService(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration, ILogger<ExternalProductionService> logger)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.ProductionService;
            _logger = logger;
        }

        public async Task SendProductionRequestAsync(Guid userId, ProductionRequest productionRequest)
        {
            var uri = $"{_baseServiceUrl}/{API_PRODUCTION_REQUEST(userId)}";

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
