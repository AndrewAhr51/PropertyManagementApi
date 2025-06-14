namespace PropertyManagementAPI.Domain.DTOs
{
    public class RegisterUserDto
    {
        public required string UserName { get; set; }  // ✅ User's chosen username
        public required string Email { get; set; }  // ✅ User's email address
        public required string Password { get; set; }  // ✅ Secure password input
        public required string Role { get; set; }  // ✅ User role (Admin, Manager, Viewer)
    }
}