using OrderService.Models.v1.DomainModels;
using OrderService.Models.v1.DTOs;

namespace OrderService.Repositories
{
    public interface IOrderRepository
    {
        void Insert(Order order);
        void Update(Order order);
        Task<Order?> GetAsync(Guid orderId);
        Task<Order?> GetAsync(Guid userId, Guid orderId);
        Task<OrderDto?> GetOrderDtoAsync(Guid userId, Guid orderId);
        Task<(List<OrderDto> Orders, int TotalRecords)> GetOrdersWithPaginationAsync(Guid userId, string? orderName, int pageNumber, int pageSize);
        Task SaveChangeAsync();
    }
}
