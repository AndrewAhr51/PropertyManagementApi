using PropertyManagementAPI.Domain.DTOs.Payments.Banking;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Infrastructure.Repositories.Payments.Banking;

namespace PropertyManagementAPI.Application.Services.Payments.Banking
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public BankAccountService(IBankAccountRepository repo)
        {
            _bankAccountRepository = repo;
        }

        public async Task<bool> AddBankAccountAsync(BankAccountDto dto)
        {
            var account = new BankAccountInfo
            {
                BankName = dto.BankName,
                AccountNumberMasked = dto.AccountNumberMasked,
                RoutingNumber = dto.RoutingNumber,
                AccountType = dto.AccountType,
                CreatedOn = DateTime.UtcNow
            };

            var save =  await _bankAccountRepository.AddBankAccountAsync(account);

            return save;

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
