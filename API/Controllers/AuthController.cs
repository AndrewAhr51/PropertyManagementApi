using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Application.Services.Auth;

namespace PropertyManagementAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // 🔹 Authenticate User
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto loginDto)
        {
            var token = await _authService.AuthenticateAsync(loginDto);
            if (token == null) return Unauthorized("Invalid credentials.");

            return Ok(new { AccessToken = token });
        }

        // 🔹 Refresh Token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string expiredToken)
        {
            var newToken = await _authService.RefreshTokenAsync(expiredToken);
            if (newToken == null) return Unauthorized("Invalid or expired refresh token.");

            return Ok(new { AccessToken = newToken });
        }

        // 🔹 Generate Password Reset Token
        [HttpPost("forgot-password")]
        public async Task<IActionResult> GenerateResetToken([FromBody] EmailDto email)
        {
            var token = await _authService.GenerateResetTokenAsync(email.EmailAddress);
            if (token == null) return NotFound("User not found.");

            return Ok(new { ResetToken = token });
        }

        // 🔹 Send Password Reset Email
        [HttpPost("send-reset-email")]
        public async Task<IActionResult> SendResetEmail([FromBody] EmailDto email)
        {
            var success = await _authService.SendResetEmailAsync(email);
            if (!success) return NotFound("User not found or email failed to send.");

            return Ok("Password reset email sent successfully.");
        }

        // Fix for CS1503: Argument 1: cannot convert from 'string' to 'PropertyManagementAPI.Domain.DTOs.EmailDto'
        // The ValidateResetTokenAsync method expects an EmailDto object as the first argument.
        // Update the code to create an EmailDto instance using the provided email string.

        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken([FromBody] ValidateResetDto validateResetDto)
        {
            var emailDto = new EmailDto { EmailAddress = validateResetDto.Email }; // Create EmailDto instance
            var isValid = await _authService.ValidateResetTokenAsync(emailDto.EmailAddress, validateResetDto.Token); // Pass EmailDto instead of string
            if (!isValid) return BadRequest("Invalid or expired reset token.");

            return Ok("Reset token is valid.");
        }

        // 🔹 Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var emailDto = new EmailDto { EmailAddress = resetPasswordDto.Email };
            var success = await _authService.ResetPasswordAsync(emailDto.EmailAddress, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!success) return BadRequest("Password reset failed.");

            return Ok("Password reset successfully.");
        }   
    }
}   