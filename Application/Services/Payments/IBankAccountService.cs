using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IBankAccountService
    {
        Task<int> AddBankAccountAsync(BankAccountDto dto);
        Task<IEnumerable<BankAccountInfo>> GetAccountsByOwnerAsync(int ownerId);
        Task<BankAccountInfo> GetByIdAsync(int bankAccountInfoId);
    }
}
