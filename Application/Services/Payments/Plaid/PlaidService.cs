using CorrelationId;
using CorrelationId.Abstractions;
using Going.Plaid;
using Going.Plaid.Auth;
using Going.Plaid.Entity;
using Going.Plaid.Item;
using Going.Plaid.Link;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Exceptions;
using PropertyManagementAPI.Infrastructure.Auditing;
using PropertyManagementAPI.Infrastructure.Data;


namespace PropertyManagementAPI.Application.Services.Payments.Plaid
{
    public class PlaidService : IPlaidService
    {
        private readonly PlaidClient _plaidClient;
        private readonly ILogger<PlaidService> _logger;
        private readonly PlaidPaymentAuditLogger _plaidAuditLogger;
        private readonly ICorrelationContextAccessor _correlation;
        


        public PlaidService(
            PlaidClient plaidClient,
            ILogger<PlaidService> logger,
            PlaidPaymentAuditLogger plaidAuditLogger,
            ICorrelationContextAccessor correlation)
        {
            _plaidClient = plaidClient;
            _logger = logger;
            _plaidAuditLogger = plaidAuditLogger;
            _correlation = correlation;
        }

        public async Task<string> CreateLinkTokenAsync()
        {
            var correlationId = _correlation.CorrelationContext?.CorrelationId;

            try
            {
                var request = new LinkTokenCreateRequest
                {
                    ClientName = "Demo Property App",
                    Language = Language.English,
                    CountryCodes = new[] { CountryCode.Us },
                    Products = new[] { Products.Auth },
                    User = new LinkTokenCreateRequestUser
                    {
                        ClientUserId = Guid.NewGuid().ToString()
                    }
                };

                var response = await _plaidClient.LinkTokenCreateAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    await _plaidAuditLogger.LogPlaidFailureAsync("LinkTokenCreate", response.Error, correlationId);
                    throw new PlaidException("Failed to create Plaid link token.", response.Error);
                }

                await _plaidAuditLogger.LogPlaidSuccessAsync("LinkTokenCreate", response.LinkToken, correlationId);
                return response.LinkToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🧨 Exception during CreateLinkTokenAsync [{CorrelationId}]", correlationId);
                throw;
            }
        }

        public async Task<string> ExchangePublicTokenAsync(string publicToken)
        {
            var correlationId = _correlation.CorrelationContext?.CorrelationId;

            try
            {
                var response = await _plaidClient.ItemPublicTokenExchangeAsync(new ItemPublicTokenExchangeRequest
                {
                    PublicToken = publicToken
                });

                if (!response.IsSuccessStatusCode)
                {
                    await _plaidAuditLogger.LogPlaidFailureAsync("TokenExchange", response.Error, correlationId);
                    throw new PlaidException("Failed to exchange public token with Plaid.", response.Error);
                }

                await _plaidAuditLogger.LogPlaidSuccessAsync("TokenExchange", response.AccessToken, correlationId);
                return response.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🧨 Exception during ExchangePublicTokenAsync [{CorrelationId}]", correlationId);
                throw;
            }
        }

        public async Task<IEnumerable<Account>> GetAccountsAsync(string accessToken)
        {
            var correlationId = _correlation.CorrelationContext?.CorrelationId;

            try
            {
                var response = await _plaidClient.AuthGetAsync(new AuthGetRequest
                {
                    AccessToken = accessToken
                });

                if (!response.IsSuccessStatusCode)
                {
                    await _plaidAuditLogger.LogPlaidFailureAsync("AuthGet", response.Error, correlationId);
                    throw new PlaidException("Failed to retrieve bank accounts from Plaid.", response.Error);
                }

                await _plaidAuditLogger.LogPlaidSuccessAsync("AuthGet", $"{response.Accounts?.Count} accounts retrieved", correlationId);
                return response.Accounts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🧨 Exception during GetAccountsAsync [{CorrelationId}]", correlationId);
                throw;
            }
        }
    }
}