namespace PropertyManagementAPI.Application.Services
{    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}
