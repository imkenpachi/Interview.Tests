using OrderService.Models.v1.Enums;

namespace OrderService.Models.v1.DTOs
{
    public class UpdateProcessJobRequest
    {
        public ProcessJobType JobType { get; set; }
        public ProcessJobStatus JobStatus { get; set; }
        public string? Note { get; set; }
    }
}
