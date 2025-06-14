namespace PropertyManagementAPI.Domain.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }  // Unique Identifier
        public required string UserName { get; set; } = string.Empty;  // ✅ Ensures non-null value
        public required string Email { get; set; } = string.Empty;  // ✅ Default empty string
        public required int Role { get; set; }  // ✅ Integer remains unchanged (Admin, Manager, Viewer)

        // 🔹 Added properties for password reset support
        public string? PasswordResetToken { get; set; }  // ✅ Token for resetting password
        public DateTime? TokenExpiration { get; set; }  // ✅ Expiration date for password reset token

        // 🔹 Added status tracking
        public bool IsActive { get; set; } = true;  // ✅ Tracks active/inactive users
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;  // ✅ Auto-initializes creation date
    }
}