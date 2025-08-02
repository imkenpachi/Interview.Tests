using PaymentService.Models.v1.DomainModels;
using PaymentService.Models.v1.DTOs;

namespace PaymentService.Repositories
{
    public interface IPaymentTransactionRepository
    {
        void Insert(PaymentTransaction paymentTransaction);
        void Update(PaymentTransaction paymentTransaction);
        Task SaveChangeAsync();
    }
}
