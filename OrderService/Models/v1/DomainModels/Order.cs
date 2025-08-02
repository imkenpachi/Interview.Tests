using OrderService.Models.v1.Common;
using OrderService.Models.v1.Enums;

namespace OrderService.Models.v1.DomainModels
{
    public class Order : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set;}
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
