using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailRepository;

        public EmailService(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        // ✅ Send an email and log it in the database
        public async Task<bool> SendEmailAsync(string to, string subject, string body, int senderId)
        {
            var email = new Emails
            {
                SenderId = senderId,
                Recipient = to,
                Subject = subject,
                Body = body,
                SentDate = DateTime.UtcNow,
                Status = "Pending"
            };

            return await _emailRepository.LogSentEmailAsync(email);
        }

        // ✅ Retrieve an email by ID
        public async Task<Emails?> GetEmailByIdAsync(int emailId)
        {
            return await _emailRepository.GetEmailByIdAsync(emailId);
        }

        // ✅ Retrieve all emails
        public async Task<IEnumerable<Emails>> GetAllEmailsAsync()
        {
            return await _emailRepository.GetAllEmailsAsync();
        }

        // ✅ Update email delivery status
        public async Task<bool> UpdateEmailStatusAsync(int emailId, bool isDelivered)
        {
            return await _emailRepository.UpdateEmailStatusAsync(emailId, isDelivered);
        }
    }
}