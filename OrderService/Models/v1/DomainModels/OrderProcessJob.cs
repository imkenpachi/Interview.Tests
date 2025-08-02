using OrderService.Models.v1.Enums;
using OrderService.Models.v1.Common;

namespace OrderService.Models.v1.DomainModels
{
    public class OrderProcessJob : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ProcessId { get; set; }
        public OrderProcess? OrderProcess { get; set; }
        public ProcessJobType ProcessJobType { get; set; }
        public ProcessJobStatus ProcessJobStatus { get; set; }
        public string? Note { get; set; }
    }
}
