using PaymentService.Models.v1.Enums;

namespace PaymentService.Models.v1.DTOs
{
    public class PaymentResponse
    {
        public required Guid PaymentId { get; set; }
        public required PaymentStatus PaymentStatus { get; set; }
    }
}
