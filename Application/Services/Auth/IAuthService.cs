using System.Threading.Tasks;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Application.Services.Auth
{
    public interface IAuthService
    {
        // ✅ Authentication Methods
        Task<string?> AuthenticateAsync(LoginDto loginDto); // ✅ Generates JWT token
        Task<string?> RefreshTokenAsync(string expiredToken); // ✅ Supports secure token renewal

        // ✅ Password Reset Methods
        Task<string?> GenerateResetTokenAsync(string email); // ✅ Creates a secure reset token
        Task<bool> SendResetEmailAsync(EmailDto emailDto); // ✅ Uses EmailDto for structured email sending
        Task<bool> ValidateResetTokenAsync(string email, string token); // ✅ Checks token validity
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword); // ✅ Performs the actual reset
        Task<bool> InvalidateResetTokenAsync(string email); // ✅ Removes reset token after use

        // ✅ Multi-Factor Authentication (MFA)
        Task<string?> GenerateMfaCodeAsync(EmailDto emailDto); // ✅ Uses EmailDto for MFA code generation
        Task<bool> ValidateMfaCodeAsync(EmailDto emailDto, string code); // ✅ Uses EmailDto for validation
        Task<bool> EnableMfaAsync(EmailDto emailDto); // ✅ Enables MFA for a user
        Task<bool> DisableMfaAsync(EmailDto emailDto); // ✅ Disables MFA for a user
    }
}