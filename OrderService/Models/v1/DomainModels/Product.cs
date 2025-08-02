using OrderService.Models.v1.Common;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models.v1.DomainModels
{
    public class Product : BaseEntity
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }
        [StringLength(255)]
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; } = [];
    }
}
