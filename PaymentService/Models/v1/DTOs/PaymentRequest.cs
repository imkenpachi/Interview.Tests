using PaymentService.Models.v1.Enums;

namespace PaymentService.Models.v1.DTOs
{
    public class PaymentRequest
    {
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
    }
}
