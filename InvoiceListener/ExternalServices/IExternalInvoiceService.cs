using InvoiceListener.Models.DTOs;

namespace InvoiceListener.ExternalServices
{
    public interface IExternalInvoiceService
    {
        Task SendInvoiceGenerationRequestAsync(Guid userId, InvoiceGenerationRequest productionRequest);
    }
}
