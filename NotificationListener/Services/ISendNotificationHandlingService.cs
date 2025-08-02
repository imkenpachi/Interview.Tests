using NotificationListener.Models.DTOs;
using NotificationListener.Models.Enums;

namespace NotificationListener.Services
{
    public interface ISendNotificationHandlingService
    {
        Task<(ResponseCode, string note)> ProcessMessageAsync(UpdateOrderPaymentStatusMessage message);
        Task ProcessExceptionAsync(Exception exception, UpdateOrderPaymentStatusMessage message);
    }
}
