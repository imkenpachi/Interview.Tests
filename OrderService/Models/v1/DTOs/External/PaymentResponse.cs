namespace OrderService.Models.v1.DTOs.External
{
    public class PaymentResponse
    {
        public required Guid PaymentId { get; set; }
        public required PaymentStatus PaymentStatus { get; set; }
    }
}
