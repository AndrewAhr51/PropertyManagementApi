using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountService(IBankAccountRepository repo)
        {
            _bankAccountRepository = repo;
        }

        public async Task<int> AddBankAccountAsync(BankAccountDto dto)
        {
            var account = new BankAccountInfo
            {
                BankName = dto.BankName,
                AccountNumberMasked = dto.AccountNumberMasked,
                RoutingNumber = dto.RoutingNumber,
                AccountType = dto.AccountType,
                CreatedOn = DateTime.UtcNow
            };

            return await _bankAccountRepository.AddBankAccountAsync(account);
        }

        public async Task<IEnumerable<BankAccountInfo>> GetAccountsByOwnerAsync(int ownerId)
        {
            return await _bankAccountRepository.GetBankAccountByOwnerAsync(ownerId);
        }

        public async Task<BankAccountInfo> GetByIdAsync(int bankAccountInfoId)
        {
            return await _bankAccountRepository.GetBankAccountByIdAsync(bankAccountInfoId);
        }
    }
}
