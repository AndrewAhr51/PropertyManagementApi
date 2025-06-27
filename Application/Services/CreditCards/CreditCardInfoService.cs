using PropertyManagementAPI.Domain.DTOs.CreditCard;
using PropertyManagementAPI.Domain.Entities;
using System.Text;

namespace PropertyManagementAPI.Application.Services.CreditCards
{
    public class CreditCardInfoService : ICreditCardInfoService
    {
        private readonly ICreditCardInfoRepository _repository;

        public CreditCardInfoService(ICreditCardInfoRepository repository)
        {
            _repository = repository;
        }

        public async Task AddCreditCardAsync(CreditCardInfoDto dto)
        {
            var cardInfo = new CreditCardInfo
            {
                TenantId = dto.TenantId,
                PropertyId = dto.PropertyId,
                CardHolderName = dto.CardHolderName.Trim(),
                CardNumber = Encoding.UTF8.GetBytes(dto.CardNumber), // Convert string to byte[]
                LastFourDigits = dto.CardNumber[^4..], // Extract last four digits
                ExpirationDate = dto.ExpirationDate,
                CVV = Encoding.UTF8.GetBytes(dto.CVV) // Convert string to byte[]
            };

            await _repository.AddCreditCardAsync(cardInfo);
        }

        public async Task<CreditCardInfoDto?> GetCreditCardAsync(int cardId)
        {
            var cardInfo = await _repository.GetCreditCardAsync(cardId);
            return cardInfo == null ? null : new CreditCardInfoDto
            {
                CardId = cardInfo.CardId,
                TenantId = cardInfo.TenantId,
                PropertyId = cardInfo.PropertyId,
                CardHolderName = cardInfo.CardHolderName,
                CardNumber = Encoding.UTF8.GetString(cardInfo.CardNumber), // Convert byte[] to string
                LastFourDigits = cardInfo.LastFourDigits,
                ExpirationDate = cardInfo.ExpirationDate,
                CVV = Encoding.UTF8.GetString(cardInfo.CVV) // Convert byte[] to string
            };
        }

        public async Task UpdateExpirationDateAsync(int cardId, string newExpirationDate)
        {
            await _repository.UpdateExpirationDateAsync(cardId, newExpirationDate);
        }
    }
}
