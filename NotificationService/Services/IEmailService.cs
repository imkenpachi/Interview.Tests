using NotificationService.Models.v1.DTOs;

namespace NotificationService.Services
{
    public interface IEmailService
    {
        Task SendEmailNotificationAsync(Guid userId, EmailNotificationRequest request);
    }
}
