using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Database;
using OrderService.Models.v1.DomainModels;

namespace OrderService.Repositories
{
    public class OrderProcessRepository : IOrderProcessRepository
    {
        private readonly DatabaseContext _databaseContext;

        public OrderProcessRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task AddAsync(OrderProcess orderProcess)
        {
            await _databaseContext.OrderProcesses.AddAsync(orderProcess);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<OrderProcess?> GetAsync(Guid processId)
        {
            return await _databaseContext.OrderProcesses
                .Include(x => x.ProcessJobs)
                .SingleOrDefaultAsync(x => x.Id == processId);
        }

        public async Task UpdateAsync(OrderProcess orderProcess)
        {
            _databaseContext.Update(orderProcess);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
