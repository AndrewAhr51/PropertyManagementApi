using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PropertyManagementAPI.Application.Services.Accounting.Quickbooks;
using PropertyManagementAPI.Domain.DTOs.Quickbooks;
using PropertyManagementAPI.Infrastructure.Repositories.Quickbooks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PropertyManagementAPI.Infrastructure.Quickbooks
{
    public class QuickBooksTokenManager : IQuickBooksTokenManager
    {
        private readonly IConfiguration _config;
        private readonly ILogger<QuickBooksTokenManager> _logger;
        private readonly IHttpClientFactory _httpFactory;
        private readonly ITokenStore _tokenStore;

        private string? _accessToken;
        private string? _refreshToken;
        private DateTime _expiresAtUtc;

        public QuickBooksTokenManager(
            IConfiguration config,
            ILogger<QuickBooksTokenManager> logger,
            IHttpClientFactory httpClientFactory,
            ITokenStore tokenStore)
        {
            _config = config;
            _logger = logger;
            _httpFactory = httpClientFactory;
            _tokenStore = tokenStore;
        }

        public async Task<string?> GetTokenAsync(string realmId)
        {
            // Optional logging context
            _logger.LogDebug("Fetching QuickBooks token for realm: {RealmId}", realmId);

            if (!string.IsNullOrWhiteSpace(_accessToken) && _expiresAtUtc > DateTime.UtcNow)
            {
                _logger.LogDebug("Returning cached access token for realm: {RealmId}", realmId);
                return _accessToken;
            }

            if (!string.IsNullOrWhiteSpace(_refreshToken))
            {
                var refreshed = await RefreshTokenAsync(_refreshToken);
                if (refreshed?.AccessToken != null)
                {
                    _accessToken = refreshed.AccessToken;
                    _expiresAtUtc = refreshed.ExpiresAtUtc;
                    _logger.LogInformation("Token successfully refreshed for realm: {RealmId}", realmId);
                    return _accessToken;
                }

                _logger.LogWarning("Token refresh failed for realm: {RealmId}", realmId);
                return null;
            }

            _logger.LogWarning("No valid QuickBooks token available for realm: {RealmId}", realmId);
            return null;
        }

        public async Task<TokenSet?> ExchangeAuthCodeForTokenAsync(string realmId, string authCode)
        {
            var clientId = _config["QB:ClientId"];
            var clientSecret = _config["QB:Secret"];
            var redirectUri = _config["QB:RedirectUri"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(redirectUri))
            {
                _logger.LogError("QuickBooks configuration missing: ClientId/Secret/RedirectUri.");
                return null;
            }

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            var client = _httpFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Basic", encoded) },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", authCode },
            { "redirect_uri", redirectUri }
        })
            };

            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Token exchange failed for realm={RealmId}: {StatusCode} - {Body}", realmId, response.StatusCode, body);
                return null;
            }

            var parsed = JsonConvert.DeserializeObject<TokenResponse>(body);
            if (parsed == null || string.IsNullOrWhiteSpace(parsed.AccessToken))
            {
                _logger.LogError("Failed to deserialize token response for realm={RealmId}", realmId);
                return null;
            }

            var tokenSet = new TokenSet
            {
                AccessToken = parsed.AccessToken,
                RefreshToken = parsed.RefreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(55)
            };

            _accessToken = tokenSet.AccessToken;
            _refreshToken = tokenSet.RefreshToken;
            _expiresAtUtc = tokenSet.ExpiresAtUtc;

            _logger.LogInformation("Token exchange succeeded for realm={RealmId}. AccessToken length: {Length}", realmId, tokenSet.AccessToken.Length);

            // Persist token by realm for future reuse
            await _tokenStore.SaveTokenAsync(realmId, tokenSet);

            return tokenSet;
        }

        public async Task<TokenSet?> RefreshTokenAsync(string refreshToken)
        {
            var clientId = _config["QB:ClientId"];
            var clientSecret = _config["QB:Secret"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                _logger.LogError("Cannot refresh token — ClientId/Secret missing.");
                return null;
            }

            var client = _httpFactory.CreateClient();
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Basic", encoded) },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken }
                })
            };

            var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Token refresh failed: {StatusCode} - {Body}", response.StatusCode, body);
                return null;
            }

            var parsed = JsonConvert.DeserializeObject<TokenResponse>(body);
            if (parsed == null || string.IsNullOrWhiteSpace(parsed.AccessToken))
            {
                _logger.LogError("Refresh response was invalid.");
                return null;
            }

            _accessToken = parsed.AccessToken;
            _refreshToken = parsed.RefreshToken;
            _expiresAtUtc = DateTime.UtcNow.AddMinutes(55);

            _logger.LogInformation("Access token refreshed.");
            return new TokenSet
            {
                AccessToken = parsed.AccessToken,
                RefreshToken = parsed.RefreshToken,
                ExpiresAtUtc = _expiresAtUtc
            };
        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; } = string.Empty;

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }
    }
}