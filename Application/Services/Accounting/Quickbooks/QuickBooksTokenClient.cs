using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using PropertyManagementAPI.Application.Configuration;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public class QuickBooksTokenClient
    {
        private readonly QuickBooksAuthSettings _settings;
        private readonly HttpClient _httpClient;

        public QuickBooksTokenClient(HttpClient httpClient, QuickBooksAuthSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
        }

        public async Task<string> GetBearerTokenAsync()
        {
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, "https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer")
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
            return doc.RootElement.GetProperty("access_token").GetString()!;
        }

    }
}
