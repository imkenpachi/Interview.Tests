using ProductionListener.Models.DTOs;

namespace ProductionListener.ExternalServices
{
    public interface IExternalOrderService
    {
        Task<OrderDto> GetOrderInfoAsync (Guid userId, Guid orderId);
        Task UpdateProcessJobStatus(Guid processId, UpdateProcessJobStatusRequest request);
    }
}
