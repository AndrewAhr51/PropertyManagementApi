using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs;

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
        public async Task<IActionResult> GenerateResetToken([FromBody] string email)
        {
            var token = await _authService.GenerateResetTokenAsync(email);
            if (token == null) return NotFound("User not found.");

            return Ok(new { ResetToken = token });
        }

        // 🔹 Send Password Reset Email
        [HttpPost("send-reset-email")]
        public async Task<IActionResult> SendResetEmail([FromBody] string email)
        {
            var success = await _authService.SendResetEmailAsync(email);
            if (!success) return NotFound("User not found or email failed to send.");

            return Ok("Password reset email sent successfully.");
        }

        // 🔹 Validate Reset Token
        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken([FromBody] ValidateResetDto validateResetDto)
        {
            var isValid = await _authService.ValidateResetTokenAsync(validateResetDto.Email, validateResetDto.Token);
            if (!isValid) return BadRequest("Invalid or expired reset token.");

            return Ok("Reset token is valid.");
        }

        // 🔹 Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var success = await _authService.ResetPasswordAsync(resetPasswordDto.Email, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (!success) return BadRequest("Password reset failed.");

            return Ok("Password reset successfully.");
        }
    }
}