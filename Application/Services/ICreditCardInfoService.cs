using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services
{
    public interface ICreditCardInfoService
    {
        Task AddCreditCardAsync(CreditCardInfoDto dto);
        Task<CreditCardInfoDto?> GetCreditCardAsync(int cardId);
        Task UpdateExpirationDateAsync(int cardId, string newExpirationDate);  // New method
    }
}
