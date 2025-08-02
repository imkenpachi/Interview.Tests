using OrderService.Models.v1.Enums;

namespace OrderService.Models.v1.DTOs
{
    public class CheckoutOrderRequest
    {
        public PaymentProvider PaymentProvider { get; set; }
    }
}
