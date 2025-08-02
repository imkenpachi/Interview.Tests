using Frontend.Enums;

namespace Frontend.Models
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAtUtc {get;set; }
        public DateTime? UpdatedAtUtc {get;set;}
        public OrderStatus Status { get; set;}
        public List<OrderDetailDto>? OrderDetails { get; set;}
    }
}
