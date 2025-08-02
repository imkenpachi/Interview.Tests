using PaymentService.Models.v1.Enums;
using PaymentService.Models.v1.Common;

namespace PaymentService.Models.v1.DomainModels
{
    public class Payment : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ExternalId { get; set; }
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public PaymentStatus Status { get; set;}
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = [];
    }
}
