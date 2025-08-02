using Frontend.Models;

namespace Frontend.ExternalServices
{
    public interface IExternalOrderService
    {
        Task<PagedResponseDto<OrderDto>> GetOrdersAsync(Guid userId, string orderName = "", int pageNumber = 1, int pageSize = 10);
        Task CheckoutOrderAsync(Guid userId, Guid orderId, CheckoutOrderRequest request);
    }
}
