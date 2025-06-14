using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Domain.Entities;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly AppDbContext _context;

        public EmailRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Log a sent email
        public async Task<bool> LogSentEmailAsync(Emails emailLog)
        {
            await _context.Emails.AddAsync(emailLog);
            return await _context.SaveChangesAsync() > 0;
        }

        // ✅ Retrieve an email log by ID
        public async Task<Emails?> GetEmailByIdAsync(int id)
        {
            return await _context.Emails.FindAsync(id);
        }

        // ✅ Update email status (Sent, Failed)
        public async Task<bool> UpdateEmailStatusAsync(int id, bool isDelivered)
        {
            var emailLog = await _context.Emails.FindAsync(id);
            if (emailLog == null) return false;

            emailLog.IsDelivered = isDelivered;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}