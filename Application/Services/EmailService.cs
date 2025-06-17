using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories;
using System.Collections.Generic;
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

        // ✅ Send an email (Simulated sending logic)
        public async Task<bool> SendEmailAsync(EmailDto emailDto)
        {
            // Simulate email sending logic (e.g., SMTP, third-party API)
            var success = true; // Assume email is sent successfully

            if (success)
            {
                return await LogSentEmailAsync(emailDto);
            }

            return false;
        }

        // ✅ Log sent email in the database
        public async Task<bool> LogSentEmailAsync(EmailDto emailDto)
        {
            var emailEntity = new Emails
            {
                SenderId = emailDto.SenderId ?? 0,
                Recipient = emailDto.EmailAddress,
                Subject = emailDto.Subject,
                Body = emailDto.Body,
                SentDate = DateTime.UtcNow,
                Status = "Sent"
            };

            return await _emailRepository.LogSentEmailAsync(emailEntity);
        }

        // ✅ Retrieve an email by ID
        public async Task<EmailDto?> GetEmailByIdAsync(int emailId)
        {
            var emailEntity = await _emailRepository.GetEmailByIdAsync(emailId);
            if (emailEntity == null) return null;

            return new EmailDto
            {
                EmailAddress = emailEntity.Recipient,
                Subject = emailEntity.Subject,
                Body = emailEntity.Body,
                SenderId = emailEntity.SenderId
            };
        }

        // ✅ Retrieve all emails
        public async Task<IEnumerable<EmailDto>> GetAllEmailsAsync()
        {
            var emailEntities = await _emailRepository.GetAllEmailsAsync();
            var emailDtos = new List<EmailDto>();

            foreach (var email in emailEntities)
            {
                emailDtos.Add(new EmailDto
                {
                    EmailAddress = email.Recipient,
                    Subject = email.Subject,
                    Body = email.Body,
                    SenderId = email.SenderId
                });
            }

            return emailDtos;
        }

        // ✅ Update email delivery status
        public async Task<bool> UpdateEmailStatusAsync(int emailId, bool isDelivered)
        {
            return await _emailRepository.UpdateEmailStatusAsync(emailId, isDelivered);
        }
    }
}