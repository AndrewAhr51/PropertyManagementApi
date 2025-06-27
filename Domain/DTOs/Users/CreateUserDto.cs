namespace PropertyManagementAPI.Domain.DTOs.Users
{
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // ✅ Will be hashed before storing
        public string Role { get; set; }
    }
}