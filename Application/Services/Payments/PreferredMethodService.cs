using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public class PreferredMethodService : IPreferredMethodService
    {
        private readonly IPreferredMethodRepository _preferredMethodRepository;
        private readonly ICardTokenRepository _cardTokenRepository;
        private readonly IBankAccountRepository _bankAccountRepository;

        public PreferredMethodService(
            IPreferredMethodRepository preferredMethodRepository,
            ICardTokenRepository cardTokenRepository,
            IBankAccountRepository bankAccountRepository)
        {
            _preferredMethodRepository = preferredMethodRepository;
            _cardTokenRepository = cardTokenRepository;
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<int> SetPreferredMethodAsync(CreatePreferredMethodDto dto)
        {
            // Clear existing default if needed
            if (dto.IsDefault)
            {
                await _preferredMethodRepository.ClearDefaultAsync(dto.TenantId, dto.OwnerId);
            }

            var method = new PreferredMethod
            {
                TenantId = dto.TenantId,
                OwnerId = dto.OwnerId,
                MethodType = dto.MethodType,
                CardTokenId = dto.CardTokenId,
                BankAccountInfoId = dto.BankAccountInfoId,
                IsDefault = dto.IsDefault,
                UpdatedOn = DateTime.UtcNow
            };

            return await _preferredMethodRepository.UpsertAsync(method);
        }

        public async Task<PreferredMethodResponseDto> GetPreferredMethodByTenantAsync(int tenantId)
        {
            var method = await _preferredMethodRepository.GetDefaultByTenantAsync(tenantId);
            return await MapToResponseDto(method);
        }

        public async Task<PreferredMethodResponseDto> GetPreferredMethodByOwnerAsync(int ownerId)
        {
            var method = await _preferredMethodRepository.GetDefaultByOwnerAsync(ownerId);
            return await MapToResponseDto(method);
        }

        private async Task<PreferredMethodResponseDto> MapToResponseDto(PreferredMethod method)
        {
            if (method == null) return null;

            var dto = new PreferredMethodResponseDto
            {
                PreferredMethodId = method.PreferredMethodId,
                MethodType = method.MethodType,
                IsDefault = method.IsDefault,
                UpdatedOn = method.UpdatedOn,
                TenantId = method.TenantId,
                OwnerId = method.OwnerId
            };

            if (method.CardTokenId.HasValue)
            {
                var card = await _cardTokenRepository.GetCardTokenByIdAsync(method.CardTokenId.Value);
                dto.Last4Digits = card?.Last4Digits;
                dto.CardBrand = card?.CardBrand;
            }

            if (method.BankAccountInfoId.HasValue)
            {
                var bank = await _bankAccountRepository.GetBankAccountByIdAsync(method.BankAccountInfoId.Value);
                dto.BankName = bank?.BankName;
                dto.AccountType = bank?.AccountType;
            }

            return dto;
        }
    }
}
