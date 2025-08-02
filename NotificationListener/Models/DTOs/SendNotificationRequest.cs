using NotificationListener.Models.Enums;

namespace NotificationListener.Models.DTOs
{
    public class SendNotificationRequest
    {
        public required string Subject { get; set; }
        public string? Body { get; set; }
        public required string To { get; set; }
        public string? Bcc { get; set; }
    }
}
