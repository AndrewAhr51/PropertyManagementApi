using PropertyManagementAPI.Application.Services.Accounting.Quickbooks;

namespace PropertyManagementAPI.Application.Services.Auth
{
    public class QuickBooksTokenManager
    {
        private readonly QuickBooksTokenClient _tokenClient;
        private string? _cachedToken;
        private DateTime _expiry;

        public QuickBooksTokenManager(QuickBooksTokenClient tokenClient)
        {
            _tokenClient = tokenClient;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _expiry)
                return _cachedToken;

            _cachedToken = await _tokenClient.GetBearerTokenAsync();
            _expiry = DateTime.UtcNow.AddMinutes(50); // Adjust as needed
            return _cachedToken;
        }
    }
}
