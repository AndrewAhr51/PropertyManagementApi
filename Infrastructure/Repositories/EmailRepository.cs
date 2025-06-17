using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly SQLServerDbContext _context;

        public EmailRepository(SQLServerDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LogSentEmailAsync(Emails emailLog)
        {
            await _context.Emails.AddAsync(emailLog);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Emails?> GetEmailByIdAsync(int emailId)
        {
            return await _context.Emails
                .Include(e => e.Sender) // ✅ Ensures User (Sender) is loaded
                .FirstOrDefaultAsync(e => e.EmailId == emailId);
        }

        public async Task<IEnumerable<Emails>> GetAllEmailsAsync()
        {
            return await _context.Emails
                .Include(e => e.Sender) // ✅ Ensures User (Sender) is loaded
                .ToListAsync();
        }

        public async Task<bool> UpdateEmailStatusAsync(int emailId, bool isDelivered)
        {
            var emailLog = await _context.Emails.FindAsync(emailId);
            if (emailLog == null) return false;

            emailLog.IsDelivered = isDelivered;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}