using NotificationService.Models.v1.DomainModels;

namespace NotificationService.Repositories
{
    public interface IEmailLogRepository
    {
        Task Insert(EmailLog emailLog);
    }
}
