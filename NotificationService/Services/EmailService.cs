using NotificationService.ExternalServices;
using NotificationService.Models.v1.DTOs;
using NotificationService.Models.v1.DTOs.External;
using NotificationService.Repositories;
using NotificationService.Models.v1.DomainModels;
using NotificationService.Configurations;

namespace NotificationService.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailLogRepository _emailLogRepository;
        private readonly IExternalEmailProvider _externalEmailProvider;
        private readonly ILogger<EmailService> _logger;
        private readonly string _fromEmail;

        public EmailService(IEmailLogRepository orderRepository, IExternalEmailProvider externalEmailProvider, IServiceConfiguration serviceConfiguration, ILogger<EmailService> logger)
        {
            _emailLogRepository = orderRepository;
            _externalEmailProvider = externalEmailProvider;
            _logger = logger;
            _fromEmail = serviceConfiguration.AppSettings.FromEmail;
        }

        public async Task SendEmailNotificationAsync(Guid userId, EmailNotificationRequest request)
        {
            var response = await _externalEmailProvider.SendEmailNotificationAsync(new SendEmailRequest
            {
                From = _fromEmail,
                Subject = request.Subject,
                To = request.To,
                Bcc = request.Bcc,
                Body = request.Body,
            });

            var emailLog = new EmailLog
            {
                UserId = userId,
                Subject = request.Subject,
                Body = request.Body,
                From = _fromEmail,
                To = request.To,
                Bcc = request.Bcc,
                TrackingId = response.TrackingId,
                CreatedBy = userId,
            };
            await _emailLogRepository.Insert(emailLog);
        }
    }
}
