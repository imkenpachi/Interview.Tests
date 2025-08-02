using ProductionListener.Models.Enums;

namespace ProductionListener.Models.DTOs
{
    public class ProductionRequest
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public required string OrderName { get; set; }
        public required string OrderStatus { get; set; }
        public List<OrderDetailDto>? OrderDetails { get; set; }
    }
}
