using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Infrastructure.Data;
using System;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.Banking
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MySqlDbContext _context;

        public BankAccountRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BankAccount>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.BankAccount
                .Where(b => b.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<bool> AddBankAccountAsync(BankAccount account)
        {
            await _context.BankAccount.AddAsync(account);
            var save = await _context.SaveChangesAsync();
            return save > 0; 
        }

        public async Task<BankAccount> GetBankAccountByIdAsync(int bankAccountId)
        {
            var bankAccount = await _context.BankAccount
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BankAccountId == bankAccountId);
            if (bankAccount == null)
            {
                throw new KeyNotFoundException($"Bank account with ID {bankAccountId} not found.");
            }
            return bankAccount;

        }

        public async Task<IEnumerable<BankAccount>> GetBankAccountByOwnerAsync(int ownerId)
        {
            var accounts = await _context.PreferredMethods
                .Where(pm => pm.OwnerId == ownerId && pm.BankAccountId != null)
                .Include(pm => pm.BankAccount)
                .Select(pm => pm.BankAccount)
                .ToListAsync();

            if (accounts == null || !accounts.Any())
            {
                throw new KeyNotFoundException($"No bank accounts found for owner with ID {ownerId}.");
            }
            return accounts;
        }

    }
}
