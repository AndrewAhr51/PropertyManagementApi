using Microsoft.AspNetCore.Identity;

namespace PropertyManagementAPI.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } // ✅ Stores refresh tokens securely
        public DateTime RefreshTokenExpiryTime { get; set; } // ✅ Tracks token expiration
    }
}