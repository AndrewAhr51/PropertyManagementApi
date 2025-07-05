using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using System;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.Banking
{
    public interface IBankAccountRepository
    {
        Task<bool> AddBankAccountAsync(BankAccount account);
        Task<BankAccount> GetBankAccountByIdAsync(int bankAccountId);
        Task<IEnumerable<BankAccount>> GetBankAccountByOwnerAsync(int ownerId);
        Task<IEnumerable<BankAccount>> GetByTenantIdAsync(int tenantId);
    }
}
