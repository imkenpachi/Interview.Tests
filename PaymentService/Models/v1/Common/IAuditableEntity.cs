namespace PaymentService.Models.v1.Common
{
    public interface IAuditableEntity
    {
        Guid CreatedBy { get; set; }
        Guid? UpdatedBy { get; set; }
        DateTime CreatedAtUtc { get; set; }
        DateTime? UpdatedAtUtc { get; set; }
    }
}
