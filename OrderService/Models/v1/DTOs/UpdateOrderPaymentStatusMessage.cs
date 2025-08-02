using OrderService.Models.v1.Enums;

namespace OrderService.Models.v1.DTOs
{
    public class UpdateOrderPaymentStatusMessage
    {
        public Guid OrderId { get; set; }
        public Guid ProcessId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
    }
}
