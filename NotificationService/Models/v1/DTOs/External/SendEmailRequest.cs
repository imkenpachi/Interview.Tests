using NotificationService.Models.v1.Enums;

namespace NotificationService.Models.v1.DTOs.External
{
    public class SendEmailRequest
    {
        public required string Subject { get; set; }
        public string? Body { get; set; }
        public required string From { get; set; }
        public required string To { get; set; }
        public string? Bcc { get; set; }
    }
}
