using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly MySqlDbContext _context;

        public PaymentRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> GetByIdAsync(int paymentId)
        {
            var payments = await _context.Payments
                .Include(p => p.Invoice) 
                .Include(p => p.Tenant)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payments == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }

            return payments;
        }

        public async Task<IEnumerable<Payment>> GetByInvoiceIdAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
