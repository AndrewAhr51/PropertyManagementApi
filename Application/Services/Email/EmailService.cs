using Microsoft.Extensions.Configuration;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories.Email;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;


namespace PropertyManagementAPI.Application.Services.Email
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _config;
        private readonly IEmailRepository _emailRepository;
        private readonly ILogger<EmailService> _logger;
        public EmailService(IConfiguration config, IEmailRepository repository, ILogger<EmailService> logger)
        {
            _config = config;
            _emailRepository = repository;
            _logger = logger;
        }

        public async Task<bool> SendInvoiceEmailAsync(string recipientEmail, string invoicePdfPath)
        {
            try
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

                // Read the PDF file as a byte array
                byte[] invoiceBlob = await File.ReadAllBytesAsync(invoicePdfPath);

                var save = await _emailRepository.LogSentEmailAsync(new EmailDto
                {
                    EmailAddress = recipientEmail,
                    Subject = "Your Invoice",
                    Body = "Please find your invoice attached.",
                    Sender = "info@propertymanagement.com",
                    AttachmentBlob = invoiceBlob
                });

                return save;
            }
            catch (Exception ex)
            {
                // Log the error (assuming you have a logging service)
                _logger.LogError($"Error sending email to {recipientEmail}: {ex.Message}", ex);

                return false; // Return false to indicate failure
            }
        }

        public async Task<bool> SendInvoiceEmailAsync(string recipientEmail, string subject, string body, string pdfPath)
        {
            try
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
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(recipientEmail);
                mailMessage.Attachments.Add(new Attachment(pdfPath));

                await smtpClient.SendMailAsync(mailMessage);

                // Read the PDF file as a byte array
                byte[] invoiceBlob = await File.ReadAllBytesAsync(pdfPath);

                var save = await _emailRepository.LogSentEmailAsync(new EmailDto
                {
                    EmailAddress = recipientEmail,
                    Subject = subject,
                    Body = body,
                    Sender = "info@propertymanagement.com",
                    AttachmentBlob = invoiceBlob
                });

                return save;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"SMTP error sending email to {recipientEmail}: {smtpEx.Message}", smtpEx);
            }
            catch (IOException ioEx)
            {
                _logger.LogError($"File error reading PDF attachment {pdfPath}: {ioEx.Message}", ioEx);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error sending email to {recipientEmail}: {ex.Message}", ex);
            }

            return false; // Return false to indicate failure
        }

        public Task<IEnumerable<EmailDto>> GetAllEmailsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<EmailDto?> GetEmailByIdAsync(int emailId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogSentEmailAsync(EmailDto emailDto)
        {
            throw new NotImplementedException();
        }

       public Task<bool> SendEmailAsync(EmailDto emailDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEmailStatusAsync(int emailId, bool isDelivered)
        {
            throw new NotImplementedException();
        }
    }
}