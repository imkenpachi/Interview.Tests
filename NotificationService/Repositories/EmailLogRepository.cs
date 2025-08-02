using Microsoft.EntityFrameworkCore;
using NotificationService.Infrastructure.Database;
using NotificationService.Models.v1.DomainModels;
using NotificationService.Models.v1.DTOs;

namespace NotificationService.Repositories
{
    public class EmailLogRepository : IEmailLogRepository
    {
        private readonly DatabaseContext _databaseContext;

        public EmailLogRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task Insert(EmailLog emailLog)
        {
            _databaseContext.EmailLogs.Add(emailLog);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
