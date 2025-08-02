using OrderService.Models.v1.Enums;

namespace OrderService.Models.v1.DTOs
{
    public class OrderStatusUpdate
    {
        public Guid OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
