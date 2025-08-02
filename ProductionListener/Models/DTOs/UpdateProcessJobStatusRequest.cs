using ProductionListener.Models.Enums;

namespace ProductionListener.Models.DTOs
{
    public class UpdateProcessJobStatusRequest
    {
        public required string JobType { get; set; }
        public required string JobStatus { get; set; }
        public string? Note { get; set; }
    }
}
