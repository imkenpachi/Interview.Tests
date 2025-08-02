using PaymentService.Models.v1.Enums;

namespace PaymentService.Models.v1.DTOs.External
{
    public class EwalletPaymentRequest
    {
        public decimal Amount { get; set; }
        public required string NotificationUri { get; set; }
        public required MerchanData MerchanData { get; set; }
    }

    public class MerchanData
    {
        public Guid OrderId { get; set; }
    }
}
