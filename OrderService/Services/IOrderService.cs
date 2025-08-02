using OrderService.Models.v1.DTOs;
using OrderService.Models.v1.Enums;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<PagedResponseDto<OrderDto>> GetOrdersAsync(Guid userId, string? orderName, int pageNumber, int pageSize);
        Task<OrderDto> GetOrderAsync(Guid userId, Guid orderId);
        Task<CheckoutOrderResponse> CheckoutOrderAsync(Guid userId, Guid orderId, CheckoutOrderRequest request);
        Task ConfirmPaymentForOrder(Guid orderId, ConfirmPaymentRequest confirmPaymentRequest);
    }
}
