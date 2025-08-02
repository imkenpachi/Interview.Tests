using NotificationListener.Models.DTOs;

namespace NotificationListener.ExternalServices
{
    public interface IExternalNotificationService
    {
        Task SendEmailNotificationRequestAsync(Guid userId, SendNotificationRequest emailRequest);
    }
}
