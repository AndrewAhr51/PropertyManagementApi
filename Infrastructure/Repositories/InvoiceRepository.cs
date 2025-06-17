using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly SQLServerDbContext _context;

    public InvoiceRepository(SQLServerDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId) =>
        await _context.Invoices.FindAsync(invoiceId);

    public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync() =>
        await _context.Invoices.ToListAsync();

    public async Task<IEnumerable<Invoice>> GetInvoicesByTenantIdAsync(int tenantId) =>
        await _context.Invoices.Where(i => i.TenantId == tenantId).ToListAsync();

    public async Task<bool> AddInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteInvoiceAsync(int invoiceId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        if (invoice == null) return false;

        _context.Invoices.Remove(invoice);
        return await _context.SaveChangesAsync() > 0;
    }
}