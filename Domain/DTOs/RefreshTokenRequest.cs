namespace PropertyManagementAPI.Domain.DTOs
{
    public class RefreshTokenRequest
    {
        public string ExpiredToken { get; set; } = string.Empty; // ✅ Defaults to empty string
    }
}
