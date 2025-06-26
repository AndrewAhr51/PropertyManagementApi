using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Repositories.Email
{
    public class EmailRepository : IEmailRepository
    {
        private readonly MySqlDbContext _context;

        public EmailRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<bool> LogSentEmailAsync(EmailDto emailLog)
        {
            var emailEntity = new Emails
            {
                Sender = emailLog.Sender,
                Recipient = emailLog.EmailAddress,
                Subject = emailLog.Subject,
                Body = emailLog.Body,
                AttachmentBlob = emailLog.AttachmentBlob,
                SentDate = DateTime.UtcNow,
                Status = emailLog.Status ?? "Sent",
                IsDelivered = emailLog.IsDelivered,
            };

            await _context.Emails.AddAsync(emailEntity);
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