using ECommerce.Common.Clients;
using ECommerce.Common.Helpers;
using Frontend.Configurations;
using Frontend.Models;
using System;
using System.Text;

namespace Frontend.ExternalServices
{
    public class ExternalOrderService : BaseExternalService, IExternalOrderService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseServiceUrl;
        private static string API_GET_ORDERS(Guid userId, string orderName, int pageNumber, int pageSize) => $"api/v1/users/{userId}/orders?orderName={orderName}&pageNumber={pageNumber}&pageSize={pageSize}";
        private static string API_POST_CHECKOUT_ORDER(Guid userId, Guid orderId) => $"api/v1/users/{userId}/orders/{orderId}/checkout";

        public ExternalOrderService(IHttpClientWrapper httpClientWrapper, IServiceConfiguration serviceConfiguration)
        {
            _httpClientWrapper = httpClientWrapper;
            _baseServiceUrl = serviceConfiguration.ExternalService.OrderService;
        }

        public async Task<PagedResponseDto<OrderDto>> GetOrdersAsync(Guid userId, string orderName = "", int pageNumber = 1, int pageSize = 10)
        {
            var uri = $"{_baseServiceUrl}/{API_GET_ORDERS(userId, orderName, pageNumber, pageSize)}";

            var response = await _httpClientWrapper.GetAsync<PagedResponseDto<OrderDto>>(uri, HttpErrorsToServiceExceptionConverter());
            return response;
        }

        public async Task CheckoutOrderAsync(Guid userId, Guid orderId, CheckoutOrderRequest request)
        {
            var uri = $"{_baseServiceUrl}/{API_POST_CHECKOUT_ORDER(userId, orderId)}";

            var contentString = Serializer.SerializeObject(request, ignoreNull: true);

            var content = new StringContent(contentString, Encoding.UTF8, JSON_CONTENT);
            var response = await _httpClientWrapper.PostAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to checkout order. Status Code: {response.StatusCode}, Error: {errorMessage}");
            }
        }
    }
}
