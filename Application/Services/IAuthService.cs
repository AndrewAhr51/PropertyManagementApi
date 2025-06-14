using System.Threading.Tasks;
using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(LoginDto loginDto);
        Task<string?> RefreshTokenAsync(string expiredToken); // ✅ Supports secure token renewal

        // Password Reset Methods
        Task<string?> GenerateResetTokenAsync(string email); // ✅ Creates a secure reset token
        Task<bool> SendResetEmailAsync(string email); // ✅ Sends a reset link via email
        Task<bool> ValidateResetTokenAsync(string email, string token); // ✅ Checks token validity
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword); // ✅ Performs the actual reset

    }

}