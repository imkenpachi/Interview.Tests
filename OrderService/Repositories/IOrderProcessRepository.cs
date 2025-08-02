using OrderService.Models.v1.DomainModels;

namespace OrderService.Repositories
{
    public interface IOrderProcessRepository
    {
        Task AddAsync(OrderProcess orderProcess);
        Task UpdateAsync(OrderProcess orderProcess);
        Task<OrderProcess?> GetAsync(Guid processId);
    }
}
