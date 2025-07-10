using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Configuration;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public class QuickBooksTokenClient
    {
        private readonly QuickBooksAuthSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<QuickBooksTokenClient> _logger;

        public QuickBooksTokenClient(
            HttpClient httpClient,
            QuickBooksAuthSettings settings,
            ILogger<QuickBooksTokenClient> logger)
        {
            _httpClient = httpClient;
            _settings = settings;
            _logger = logger;
        }

        public async Task<string> GetBearerTokenAsync()
        {
            const int maxRetries = 3;
            const int baseDelayMs = 500;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var credentials = Convert.ToBase64String(
                        Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));

                    var request = new HttpRequestMessage(HttpMethod.Post,
                        "https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer")
                    {
                        Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                })
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    var response = await _httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);

                    var token = doc.RootElement.GetProperty("access_token").GetString();

                    if (string.IsNullOrWhiteSpace(token))
                        throw new InvalidOperationException("Access token not found in response.");

                    _logger.LogInformation("Successfully retrieved QuickBooks access token.");
                    return token;
                }
                catch (Exception ex) when (IsTransient(ex) && attempt < maxRetries)
                {
                    int delay = baseDelayMs * (int)Math.Pow(2, attempt - 1);
                    _logger.LogWarning(ex, "Attempt {Attempt} to retrieve QuickBooks access token failed. Retrying in {Delay}ms...",
                        attempt, delay);
                    await Task.Delay(delay);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Final failure retrieving QuickBooks access token after {MaxRetries} attempts.", maxRetries);
                    throw;
                }
            }

            throw new InvalidOperationException("Token retrieval failed after maximum retry attempts.");
        }

        private bool IsTransient(Exception ex)
        {
            return ex is TimeoutException || ex is HttpRequestException || ex is OperationCanceledException;
        }
    }
}