using Intuit.Ipp.OAuth2PlatformClient;
using PropertyManagementAPI.Domain.DTOs.Quickbooks;
using TokenResponse = Intuit.Ipp.OAuth2PlatformClient.TokenResponse;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public interface IQuickBooksTokenManager
    {
        /// <summary>
        /// Retrieves the current access token from secure storage or cache.
        /// </summary>
        /// <returns>Access token string</returns>
        Task<string?> GetTokenAsync(string realmId);

        /// <summary>
        /// Exchanges an OAuth authorization code for access and refresh tokens.
        /// </summary>
        /// <param name="authorizationCode">The code received from QuickBooks OAuth</param>
        /// <returns>Token payload with access and refresh tokens</returns>
        Task<TokenSet?> ExchangeAuthCodeForTokenAsync(string realmid, string authCode);

        /// <summary>
        /// Refreshes an expired access token using a stored refresh token.
        /// </summary>
        /// <param name="refreshToken">Existing refresh token</param>
        /// <returns>New token set</returns>
        Task<TokenSet?> RefreshTokenAsync(string refreshToken);
    }
}