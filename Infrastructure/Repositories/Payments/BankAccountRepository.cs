using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MySqlDbContext _context;

        public BankAccountRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddBankAccountAsync(BankAccountInfo account)
        {
            await _context.BankAccountInfo.AddAsync(account);
            await _context.SaveChangesAsync();
            return account.BankAccountInfoId;
        }

        public async Task<BankAccountInfo> GetBankAccountByIdAsync(int bankAccountInfoId)
        {
            var bankAccountInfo =  await _context.BankAccountInfo
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BankAccountInfoId == bankAccountInfoId);
            if (bankAccountInfo == null)
                {
                throw new KeyNotFoundException($"Bank account with ID {bankAccountInfoId} not found.");
            }
            return bankAccountInfo;

        }

        public async Task<IEnumerable<BankAccountInfo>> GetBankAccountByOwnerAsync(int ownerId)
        {
            var accounts = await _context.PreferredMethods
                .Where(pm => pm.OwnerId == ownerId && pm.BankAccountInfoId != null)
                .Include(pm => pm.BankAccountInfo)
                .Select(pm => pm.BankAccountInfo)
                .ToListAsync();

            if (accounts == null || !accounts.Any())
                {
                throw new KeyNotFoundException($"No bank accounts found for owner with ID {ownerId}.");
            }
            return accounts;
        }
    }
}
