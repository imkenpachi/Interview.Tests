namespace EwalletProviderMock.Models
{
    public class CreatePaymentRequest
    {
        public decimal Amount { get; set; }
        public required string NotificationUri { get; set; }
        public MerchanData MerchanData { get; set; }
    }

    public class MerchanData
    {
        public required Guid OrderId { get;set; }
    }
}
