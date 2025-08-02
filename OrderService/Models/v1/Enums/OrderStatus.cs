namespace OrderService.Models.v1.Enums
{
    public enum OrderStatus
    {
        Created,
        PaymentPending,
        PaymentSuccess,
        PaymentFailed,
        Cancelled,
        Delivered,
    }
}
