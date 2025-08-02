
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models.v1.Common
{
    public abstract class BaseEntity : IAuditableEntity
    {
        [Required]
        public Guid CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        [Required]
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
