using InvoiceListener.Models.DTOs;
using InvoiceListener.Models.Enums;

namespace InvoiceListener.Services
{
    public interface IInvoiceGenerationHandlingService
    {
        Task<(ResponseCode, string note)> ProcessMessageAsync(UpdateOrderPaymentStatusMessage message);
        Task ProcessExceptionAsync(Exception exception, UpdateOrderPaymentStatusMessage message);
    }
}
