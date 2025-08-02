using NotificationListener.Models.Enums;

namespace NotificationListener.Models.DTOs
{
    public class InvoiceGenerationRequest
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public required string OrderName { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderDetailDto>? OrderDetails { get; set; }
    }
}
