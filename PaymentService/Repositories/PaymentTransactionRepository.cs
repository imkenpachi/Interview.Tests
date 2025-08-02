using PaymentService.Infrastructure.Database;
using PaymentService.Models.v1.DomainModels;

namespace PaymentService.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly DatabaseContext _databaseContext;

        public PaymentTransactionRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<PaymentTransaction?> GetAsync(Guid paymentId)
        {
            return await _databaseContext.PaymentTransactions.FindAsync(paymentId);
        }

        public void Insert(PaymentTransaction paymentTransaction)
        {
            _databaseContext.PaymentTransactions.Add(paymentTransaction);
        }

        public void Update(PaymentTransaction paymentTransaction)
        {
            _databaseContext.PaymentTransactions.Update(paymentTransaction);
        }

        public async Task SaveChangeAsync()
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}
