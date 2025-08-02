using PaymentService.Models.v1.DomainModels;
using PaymentService.Models.v1.DTOs;

namespace PaymentService.Repositories
{
    public interface IPaymentRepository
    {
        void Insert(Payment order);
        void Update(Payment order);
        Task<Payment?> GetAsync(Guid paymentId);
        Task<Payment?> GetByExternalIdAsync(Guid externalId);
        Task SaveChangeAsync();
    }
}
