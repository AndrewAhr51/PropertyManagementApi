using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using System;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.Banking
{
    public interface IBankAccountRepository
    {
        Task<int> AddBankAccountAsync(BankAccountInfo account);
        Task<BankAccountInfo> GetBankAccountByIdAsync(int bankAccountInfoId);
        Task<IEnumerable<BankAccountInfo>> GetBankAccountByOwnerAsync(int ownerId);
        Task<IEnumerable<BankAccount>> GetByTenantIdAsync(int tenantId);
        Task<BankAccount> AddAsync(BankAccount account);
    }
}
