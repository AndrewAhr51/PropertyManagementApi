using Microsoft.Extensions.Options;
using PropertyManagementAPI.Application.Configuration;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public class QuickBooksUrlService : IQuickBooksUrlService
    {
        private readonly QuickBooksAuthSettings _settings;

        public QuickBooksUrlService(IOptions<QuickBooksAuthSettings> options)
        {
            _settings = options.Value;
        }

        public string GetAuthorizationUrl(int tenantId)
        {
            var redirect = Uri.EscapeDataString(_settings.RedirectUri ?? "");
            var scopes = Uri.EscapeDataString("com.intuit.quickbooks.accounting");
            return $"https://appcenter.intuit.com/connect/oauth2?" +
                   $"client_id={_settings.ClientId}&" +
                   $"redirect_uri={redirect}&" +
                   $"response_type=code&" +
                   $"scope={scopes}&" +
                   $"state={tenantId}";
        }
    }
}
