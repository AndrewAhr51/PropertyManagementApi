namespace PropertyManagementAPI.Domain.DTOs.Users
{
    public class LoginDto
    {
        public required string UserName { get; set; } = string.Empty; // ✅ Defaults to empty string
        public required string Password { get; set; } = string.Empty; // ✅ Defaults to empty string
    }
}
