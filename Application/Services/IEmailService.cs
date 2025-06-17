using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Application.Services;

public interface IEmailService
{
    // ✅ Send an email and log it in the database
    Task<bool> SendEmailAsync(string to, string subject, string body, int senderId);

    // ✅ Retrieve an email by ID
    Task<Emails?> GetEmailByIdAsync(int emailId);

    // ✅ Retrieve all emails
    Task<IEnumerable<Emails>> GetAllEmailsAsync();

    // ✅ Update email delivery status
    Task<bool> UpdateEmailStatusAsync(int emailId, bool isDelivered);
}

