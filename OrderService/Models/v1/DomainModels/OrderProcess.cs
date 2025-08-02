using OrderService.Models.v1.Common;
using OrderService.Models.v1.Enums;

namespace OrderService.Models.v1.DomainModels
{
    public class OrderProcess : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string? Note { get; set; }
        public ProcessStatus Status { get; set; }

        public ICollection<OrderProcessJob> ProcessJobs { get; set; } = [];
    }
}
