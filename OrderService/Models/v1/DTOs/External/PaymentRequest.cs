using OrderService.Models.v1.Enums;

namespace OrderService.Models.v1.DTOs.External
{
    public class PaymentRequest
    {
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public required string PaymentMethod { get; set; }
    }
}
