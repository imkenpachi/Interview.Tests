using PaymentService.Models.v1.Common;

namespace PaymentService.Models.v1.DomainModels
{
    public class PaymentTransaction : BaseEntity
    {
        public Guid PaymentId { get; set; }
        public Payment? Payment { get; set; }
        public Guid TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionTime { get; set; }
    }
}
