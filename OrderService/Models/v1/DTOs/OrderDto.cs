using OrderService.Models.v1.Enums;

namespace OrderService.Models.v1.DTOs
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
