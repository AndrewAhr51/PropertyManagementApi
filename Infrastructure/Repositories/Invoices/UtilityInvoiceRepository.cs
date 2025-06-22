using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Application.Repositories.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class UtilityInvoiceRepository : IUtilityInvoiceRepository
    {
        private readonly MySqlDbContext _context;

        public UtilityInvoiceRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(UtilityInvoice invoice)
        {
            _context.UtilityInvoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<UtilityInvoice?> GetByIdAsync(int invoiceId)
        {
            return await _context.UtilityInvoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task<IEnumerable<UtilityInvoice>> GetAllAsync()
        {
            return await _context.UtilityInvoices.ToListAsync();
        }

        public async Task UpdateAsync(UtilityInvoice invoice)
        {
            _context.UtilityInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int invoiceId)
        {
            var invoice = await _context.UtilityInvoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.UtilityInvoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }
    }
}