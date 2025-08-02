namespace NotificationService.Models.v1.DTOs
{
    public class EmailNotificationRequest
    {
        public required string Subject { get; set; }
        public string? Body { get; set; }
        public required string To { get; set; }
        public string? Bcc { get; set; }
    }
}
