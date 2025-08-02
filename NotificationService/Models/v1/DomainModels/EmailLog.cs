using NotificationService.Models.v1.Common;

namespace NotificationService.Models.v1.DomainModels
{
    public class EmailLog : BaseEntity
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TrackingId { get; set; }
        public required string Subject { get; set; }
        public string? Body { get; set; }
        public required string From { get; set; }
        public required string To { get; set; }
        public string? Bcc { get; set; }
    }
}
