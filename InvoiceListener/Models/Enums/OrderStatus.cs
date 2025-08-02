namespace InvoiceListener.Models.Enums
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
