using ECommerce.Common.Clients;
using NotificationService.Models.v1.DTOs.External;

namespace NotificationService.ExternalServices
{
    public interface IExternalEmailProvider
    {
        Task<SendEmailResponse> SendEmailNotificationAsync(SendEmailRequest paymentRequest);
    }
}
