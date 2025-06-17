using PropertyManagementAPI.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public interface IEmailRepository
    {
        Task<bool> LogSentEmailAsync(Emails emailLog);
        Task<Emails?> GetEmailByIdAsync(int emailId);
        Task<IEnumerable<Emails>> GetAllEmailsAsync();
        Task<bool> UpdateEmailStatusAsync(int emailId, bool isDelivered);
    }
}