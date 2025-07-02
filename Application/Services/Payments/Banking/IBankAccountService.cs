using PropertyManagementAPI.Domain.DTOs.Payments.Banking;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;

namespace PropertyManagementAPI.Application.Services.Payments.Banking
{
    public interface IBankAccountService
    {
        Task<int> AddBankAccountAsync(BankAccountDto dto);
        Task<IEnumerable<BankAccountInfo>> GetAccountsByOwnerAsync(int ownerId);
        Task<BankAccountInfo> GetByIdAsync(int bankAccountInfoId);
    }
}
