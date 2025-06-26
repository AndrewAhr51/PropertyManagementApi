using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Repositories.Email
{
    public interface IEmailRepository
    {
        Task<bool> LogSentEmailAsync(EmailDto emailLog);
        Task<Emails?> GetEmailByIdAsync(int emailId);
        Task<IEnumerable<Emails>> GetAllEmailsAsync();
        Task<bool> UpdateEmailStatusAsync(int emailId, bool isDelivered);
    }
}