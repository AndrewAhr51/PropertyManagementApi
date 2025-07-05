using PropertyManagementAPI.Domain.DTOs.Quickbooks;
namespace PropertyManagementAPI.Application.Services.Auth
{
    public interface IQuickBooksAuthService
    {
        Task<TokenResponse> ExchangeAuthCodeForTokenAsync(string code);
    }

}
