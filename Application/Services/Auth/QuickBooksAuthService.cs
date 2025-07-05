using PropertyManagementAPI.Domain.DTOs.Quickbooks;
using System.Text.Json;

namespace PropertyManagementAPI.Application.Services.Auth
{
    public class QuickBooksAuthService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;

        public QuickBooksAuthService(IConfiguration configuration)
        {
            _clientId = configuration["QUICKBOOKS_CLIENT_ID"] ?? throw new InvalidOperationException("Client ID missing");
            _clientSecret = configuration["QUICKBOOKS_CLIENT_SECRET"] ?? throw new InvalidOperationException("Client Secret missing");
            _redirectUri = configuration["QUICKBOOKS_REDIRECT_URI"] ?? throw new InvalidOperationException("Redirect URI missing");
        }

        public async Task<TokenResponse> ExchangeAuthCodeForTokenAsync(string code)
        {
            var client = new HttpClient();

            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", _redirectUri),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret)
            });

            var response = await client.PostAsync("https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer", requestBody);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"QuickBooks token exchange failed: {content}");

            return JsonSerializer.Deserialize<TokenResponse>(content);
        }
    }
}
