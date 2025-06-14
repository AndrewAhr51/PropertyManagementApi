using PropertyManagementAPI.Domain.Entities;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public interface IEmailRepository
    {
        Task<bool> LogSentEmailAsync(Emails emailLog);  // ✅ Store email details
        Task<Emails?> GetEmailByIdAsync(int id);  // ✅ Retrieve email log by ID
        Task<bool> UpdateEmailStatusAsync(int id, bool isDelivered);  // ✅ Update delivery status
    }
}
