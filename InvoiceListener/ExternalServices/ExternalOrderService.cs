using ECommerce.Common.Clients;
using ECommerce.Common.Exceptions;
using ECommerce.Common.Helpers;
using InvoiceListener.Configurations;
using InvoiceListener.Models.DTOs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace InvoiceListener.ExternalServices
{
    public class ExternalOrderService : BaseExternalService, IExternalOrderService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private readonly ILogger<ExternalOrderService> _logger;
        private static string API_GET_ORDER_INFO(Guid userId, Guid orderId) => $"api/internal/v1/users/{userId}/orders/{orderId}";
        private static string API_UPDATE_PROCESS_JOB_STATUS(Guid processId) => $"api/internal/v1/orderprocesses/{processId}/status";

        public ExternalOrderService(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration, ILogger<ExternalOrderService> logger)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.OrderService;
            _logger = logger;
        }

        public async Task UpdateProcessJobStatus(Guid processId, UpdateProcessJobStatusRequest request)
        {
            var uri = $"{_baseServiceUrl}/{API_UPDATE_PROCESS_JOB_STATUS(processId)}";

            var contentString = Serializer.SerializeObject(request, ignoreNull: true);

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

        public async Task<OrderDto> GetOrderInfoAsync(Guid userId, Guid orderId)
        {
            var uri = $"{_baseServiceUrl}/{API_GET_ORDER_INFO(userId, orderId)}";
            var response = await _httpClientWrapper.GetAsync<OrderDto>(uri, HttpErrorsToServiceExceptionConverter());

            return response;
        }
    }
}
