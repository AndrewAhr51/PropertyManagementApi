using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Infrastructure.Data;
using System;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.Banking
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly MySqlDbContext _context;

        public PaymentTransactionRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTransactions> LogAsync(PaymentTransactions txn)
        {
            _context.PaymentTransactions.Add(txn);
            await _context.SaveChangesAsync();
            return txn;
        }

        public async Task<IEnumerable<PaymentTransactions>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.PaymentTransactions
                .Where(t => t.TenantId == tenantId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
