using OrderService.Models.v1.Common;

namespace OrderService.Models.v1.DomainModels
{
    public class OrderDetail : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
