namespace InvoiceListener.Models.Enums
{
    public enum ResponseCode
    {
        UnknownError = -1,
        Success = 0,
        ExternalServiceCallFailed,
        InvalidMessage,
    }
}
