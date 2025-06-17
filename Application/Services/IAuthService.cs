using System.Threading.Tasks;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Application.Services
{
    public interface IAuthService
    {
        // ✅ Authentication Methods
        Task<string?> AuthenticateAsync(LoginDto loginDto); // ✅ Generates JWT token
        Task<string?> RefreshTokenAsync(string expiredToken); // ✅ Supports secure token renewal

        // ✅ Password Reset Methods
        Task<string?> GenerateResetTokenAsync(EmailDto email); // ✅ Creates a secure reset token
        Task<bool> SendResetEmailAsync(EmailDto email); // ✅ Sends a reset link via email
        Task<bool> ValidateResetTokenAsync(EmailDto email, string token); // ✅ Checks token validity
        Task<bool> ResetPasswordAsync(EmailDto email, string token, string newPassword); // ✅ Performs the actual reset
        Task<bool> InvalidateResetTokenAsync(EmailDto email); // ✅ Removes reset token after use

        // ✅ Multi-Factor Authentication (MFA)
        Task<string?> GenerateMfaCodeAsync(EmailDto email); // ✅ Generates a temporary MFA code
        Task<bool> ValidateMfaCodeAsync(EmailDto email, string code); // ✅ Validates MFA code
        Task<bool> EnableMfaAsync(EmailDto email); // ✅ Enables MFA for a user
        Task<bool> DisableMfaAsync(EmailDto email); // ✅ Disables MFA for a user
    }
}