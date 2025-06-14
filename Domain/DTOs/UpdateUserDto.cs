namespace PropertyManagementAPI.Domain.DTOs
{
    public class UpdateUserDto
    {
        public required string Username { get; set; } = string.Empty; // ✅ Defaults to empty string
        public required string Email { get; set; } = string.Empty; // ✅ Defaults to empty string
        public string Password { get; set; } = string.Empty; // ✅ Defaults to empty string (Nullable to allow updates without changing password)
    }
}
