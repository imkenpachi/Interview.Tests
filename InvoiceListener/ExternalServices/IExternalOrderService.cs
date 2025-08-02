using InvoiceListener.Models.DTOs;

namespace InvoiceListener.ExternalServices
{
    public interface IExternalOrderService
    {
        Task<OrderDto> GetOrderInfoAsync (Guid userId, Guid orderId);
        Task UpdateProcessJobStatus(Guid processId, UpdateProcessJobStatusRequest request);
    }
}
