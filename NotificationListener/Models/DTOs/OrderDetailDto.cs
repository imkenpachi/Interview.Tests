namespace NotificationListener.Models.DTOs
{
    public class OrderDetailDto
    {
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
