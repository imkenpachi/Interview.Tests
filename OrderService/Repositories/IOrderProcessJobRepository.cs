using OrderService.Models.v1.DomainModels;
using OrderService.Models.v1.Enums;

namespace OrderService.Repositories
{
    public interface IOrderProcessJobRepository
    {
        Task AddAsync(OrderProcessJob orderProcessJob);
        void Update(OrderProcessJob orderProcessJob);
        Task<OrderProcessJob> GetAsync(Guid processId, ProcessJobType jobType);
        Task SaveChangeAsync();
    }
}
