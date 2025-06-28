using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IBankAccountRepository
    {
        Task<int> AddBankAccountAsync(BankAccountInfo account);
        Task<BankAccountInfo> GetBankAccountByIdAsync(int bankAccountInfoId);
        Task<IEnumerable<BankAccountInfo>> GetBankAccountByOwnerAsync(int ownerId);
    }
}
