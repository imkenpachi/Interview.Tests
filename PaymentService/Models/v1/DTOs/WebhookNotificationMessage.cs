namespace PaymentService.Models.v1.DTOs
{
    public class WebhookNotificationMessage
    {
        public required Guid PaymentRequestId { get; set; }
        public decimal TotalAmount { get; set; }
        public required string StatusCode { get; set; }
        public Transaction? Transaction { get; set; }
    }

    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid OrderId { get; set; }
        public DateTime TransactionTime { get; set; }
    }
}
