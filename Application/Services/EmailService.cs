using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;


namespace PropertyManagementAPI.Application.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendInvoiceEmailAsync(string recipientEmail, string invoicePdfPath)
        {
            var smtpClient = new SmtpClient(_config["SmtpSettings:Host"])
            {
                Port = int.Parse(_config["SmtpSettings:Port"]),
                Credentials = new NetworkCredential(_config["SmtpSettings:Username"], _config["SmtpSettings:Password"]),
                EnableSsl = bool.Parse(_config["SmtpSettings:EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("invoices@example.com"),
                Subject = "Your Invoice",
                Body = "Please find your invoice attached.",
                IsBodyHtml = true
            };

            mailMessage.To.Add(recipientEmail);
            mailMessage.Attachments.Add(new Attachment(invoicePdfPath));

            await smtpClient.SendMailAsync(mailMessage);
        }

        Task<IEnumerable<EmailDto>> IEmailService.GetAllEmailsAsync()
        {
            throw new NotImplementedException();
        }

        Task<EmailDto?> IEmailService.GetEmailByIdAsync(int emailId)
        {
            throw new NotImplementedException();
        }

        Task<bool> IEmailService.LogSentEmailAsync(EmailDto emailDto)
        {
            throw new NotImplementedException();
        }

        Task<bool> IEmailService.SendEmailAsync(EmailDto emailDto)
        {
            throw new NotImplementedException();
        }

        Task<bool> IEmailService.UpdateEmailStatusAsync(int emailId, bool isDelivered)
        {
            throw new NotImplementedException();
        }
    }
}