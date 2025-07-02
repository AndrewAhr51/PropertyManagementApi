using PropertyManagementAPI.Domain.DTOs.Payments.CardTokens;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Repositories.Payments.CardTokens;

namespace PropertyManagementAPI.Application.Services.Payments.CardTokens
{
    public class CardTokenService : ICardTokenService
    {
        private readonly ICardTokenRepository _cardTokenRepository;

        public CardTokenService(ICardTokenRepository cardTokenRepository)
        {
            _cardTokenRepository = cardTokenRepository;
        }

        public async Task<int> CreateCardTokenAsync(CreateCardTokenDto dto)
        {
            if (dto.IsDefault)
            {
                await _cardTokenRepository.ClearCardTokenDefaultAsync(dto.TenantId, dto.OwnerId);
            }

            var token = new CardToken
            {
                TokenValue = dto.TokenValue,
                CardBrand = dto.CardBrand,
                Last4Digits = dto.Last4Digits,
                Expiration = dto.Expiration,
                TenantId = dto.TenantId,
                OwnerId = dto.OwnerId,
                IsDefault = dto.IsDefault,
                LinkedOn = DateTime.UtcNow
            };

            return await _cardTokenRepository.AddCardTokenAsync(token);
        }

        public async Task<IEnumerable<CardToken>> GetTokensByTenantAsync(int tenantId)
        {
            return await _cardTokenRepository.GetCardTokenByTenantAsync(tenantId);
        }

        public async Task SetDefaultTokenAsync(int cardTokenId, int tenantId)
        {
            await _cardTokenRepository.SetCardTokenDefaultAsync(cardTokenId, tenantId);
        }
    }
}
