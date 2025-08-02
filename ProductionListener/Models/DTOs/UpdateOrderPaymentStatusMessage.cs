using ProductionListener.Models.Enums;

namespace ProductionListener.Models.DTOs
{
    public class UpdateOrderPaymentStatusMessage
    {
        public Guid OrderId { get; set; }
        public Guid ProcessId { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; }
    }
}
