using PropertyManagementAPI.Domain.DTOs.Users;

namespace PropertyManagementAPI.Domain.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;  // ✅ JWT token for authentication
        public DateTime Expiration { get; set; }  // ✅ Token expiration time
        public bool IsSuccess { get; set; }  // ✅ Indicates if authentication was successful
        public string Message { get; set; } = string.Empty;  // ✅ Message for login status
        public UserDto? User { get; set; }  // ✅ Contains user details upon successful login
    }
}