namespace Frontend.Models
{
    public class OrderDetailDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }

    }
}
