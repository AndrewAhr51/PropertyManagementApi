namespace PropertyManagementAPI.Domain.DTOs.Users
{
    public class ResetPasswordDto
    {
        public required string Email { get; set; }  // ✅ User's email address
        public required string Token { get; set; }  // ✅ Password reset token
        public required string NewPassword { get; set; }  // ✅ New password to be set
    }
}