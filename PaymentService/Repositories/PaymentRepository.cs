using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructure.Database;
using PaymentService.Models.v1.DomainModels;

namespace PaymentService.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DatabaseContext _databaseContext;

        public PaymentRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<Payment?> GetAsync(Guid paymentId)
        {
            return await _databaseContext.Payments.FindAsync(paymentId);
        }

        public async Task<Payment?> GetByExternalIdAsync(Guid externalId)
        {
            return await _databaseContext.Payments.FirstOrDefaultAsync(p=>p.ExternalId == externalId);
        }

        public void Insert(Payment order)
        {
            _databaseContext.Payments.Add(order);
        }

        public void Update(Payment order)
        {
            _databaseContext.Payments.Update(order);
        }

        public async Task SaveChangeAsync()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}
