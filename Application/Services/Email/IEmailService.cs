﻿using System.Threading.Tasks;
using PropertyManagementAPI.Domain.DTOs.Users;

namespace PropertyManagementAPI.Application.Services.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailDto emailDto); // ✅ Sends an email
        Task<bool> LogSentEmailAsync(EmailDto emailDto); // ✅ Logs email in the database
        Task<EmailDto?> GetEmailByIdAsync(int emailId); // ✅ Retrieves an email by ID
        Task<IEnumerable<EmailDto>> GetAllEmailsAsync(); // ✅ Retrieves all emails
        Task<bool> UpdateEmailStatusAsync(int emailId, bool isDelivered); // ✅ Updates email delivery status
        Task <bool>SendInvoiceEmailAsync(string recipientEmail, string subject, string body, string pdfPath);
        Task<bool> SendInvoiceEmailAsync(string recipientEmail, string invoicePdfPath);
    }
}