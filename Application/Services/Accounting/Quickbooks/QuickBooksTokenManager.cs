using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public class QuickBooksTokenManager
    {
        private readonly IConfiguration _config;
        private readonly ILogger<QuickBooksTokenManager> _logger;
        private readonly HttpClient _http;
        private string? _lastAccessToken;


        public QuickBooksTokenManager(
            IConfiguration config,
            ILogger<QuickBooksTokenManager> logger,
            IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _logger = logger;
            _http = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Exchanges an authorization code for QuickBooks access and refresh tokens.
        /// </summary>
        public async Task<TokenResponse?> ExchangeAuthCodeForTokenAsync(string authCode)
        {
            var clientId = _config["QB:ClientId"];
            var clientSecret = _config["QB:Secret"];
            var redirectUri = _config["QB:RedirectUri"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(redirectUri))
            {
                _logger.LogError("QuickBooks credentials missing from configuration.");
                return null;
            }

            var credentials = $"{clientId}:{clientSecret}";
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authCode },
                { "redirect_uri", redirectUri }
            });

            try
            {
                var response = await _http.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("QuickBooks token exchange failed: {StatusCode} - {Body}", response.StatusCode, responseContent);
                    return null;
                }

                var token = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
                if (token == null || string.IsNullOrWhiteSpace(token.AccessToken))
                {
                    _logger.LogError("QuickBooks token deserialization failed.");
                    return null;
                }

                _lastAccessToken = token.AccessToken;
                _logger.LogInformation("QuickBooks token exchange successful. AccessToken length: {Length}", token.AccessToken.Length);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QuickBooks token exchange threw an exception.");
                return null;
            }
        }

        public Task<string?> GetTokenAsync()
        {
            return Task.FromResult(_lastAccessToken);
        }

    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}