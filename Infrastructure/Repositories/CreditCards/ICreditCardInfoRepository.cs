using PropertyManagementAPI.Domain.Entities;

public interface ICreditCardInfoRepository
{
    Task AddCreditCardAsync(CreditCardInfo creditCard);
    Task<CreditCardInfo?> GetCreditCardAsync(int cardId);
    Task UpdateExpirationDateAsync(int cardId, string newExpirationDate);  // New method
}