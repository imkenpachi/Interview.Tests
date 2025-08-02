using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Database;
using OrderService.Models.v1.DomainModels;
using OrderService.Models.v1.Enums;

namespace OrderService.Repositories
{
    public class OrderProcessJobRepository : IOrderProcessJobRepository
    {
        private readonly DatabaseContext _databaseContext;

        public OrderProcessJobRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task AddAsync(OrderProcessJob orderProcessJob)
        {
            await _databaseContext.OrderProcessJobs.AddAsync(orderProcessJob);
        }

        public async Task<OrderProcessJob> GetAsync(Guid processId, ProcessJobType jobType)
        {
            return await _databaseContext.OrderProcessJobs.FirstAsync(x => x.ProcessId == processId && x.ProcessJobType == jobType);
        }

        public void Update(OrderProcessJob orderProcessJob)
        {
            _databaseContext.OrderProcessJobs.Update(orderProcessJob);
        }

        public async Task SaveChangeAsync()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}
