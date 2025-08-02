using InvoiceListener.Models.Enums;

namespace InvoiceListener.Models.DTOs
{
    public class UpdateProcessJobStatusRequest
    {
        public required string JobType { get; set; }
        public required string JobStatus { get; set; }
        public string? Note { get; set; }
    }
}
