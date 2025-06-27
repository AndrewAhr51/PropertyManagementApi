namespace PropertyManagementAPI.Domain.DTOs.Users
{
    public class RefreshTokenRequest
    {
        public string ExpiredToken { get; set; } = string.Empty; // ✅ Defaults to empty string
    }
}
