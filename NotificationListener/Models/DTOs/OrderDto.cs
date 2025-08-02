using NotificationListener.Models.Enums;

namespace NotificationListener.Models.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderDetailDto>? OrderDetails { get; set; }
    }
}
