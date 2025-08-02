using NotificationListener.Models.DTOs;

namespace NotificationListener.ExternalServices
{
    public interface IExternalOrderService
    {
        Task<OrderDto> GetOrderInfoAsync (Guid userId, Guid orderId);
        Task UpdateProcessJobStatus(Guid processId, UpdateProcessJobStatusRequest request);
    }
}
