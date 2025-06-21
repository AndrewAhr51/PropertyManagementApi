using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository<Invoice>
{
    private readonly MySqlDbContext _context;
    private readonly ILogger<InvoiceRepository> _logger;

    public InvoiceRepository(MySqlDbContext context, ILogger<InvoiceRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId)
    {
        try
        {
            return await _context.RentInvoice.FindAsync(invoiceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoice by ID {InvoiceId}", invoiceId);
            throw;
        }
    }

    public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
    {
        try
        {
            return await _context.Invoices.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all invoices");
            return Enumerable.Empty<Invoice>();
        }
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByPropertyIdAsync(int propertyId)
    {
        try
        {
            return await _context.Invoices
                .Where(i => i.PropertyId == propertyId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving invoices for property Id {propertyId}", propertyId);
            return Enumerable.Empty<Invoice>();
        }
    }

    public async Task<bool> CreateRentalInvoiceAsync(InvoiceRentalCreateDto invoiceRent)
    {
        try
        {
            if (!await _context.Property.AnyAsync(t => t.PropertyId == invoiceRent.PropertyId))
            {
                _logger.LogWarning("PropertyId {PropertyId} not found", invoiceRent.PropertyId);
                return false;
            }

            var lease = await GetLeaseInformationAsync(invoiceRent.PropertyId);
            if (lease == null)
            {
                _logger.LogWarning("No active lease found for PropertyId {PropertyId}", invoiceRent.PropertyId);
                return false;
            }

            var invoiceTypeExists = await _context.LkupInvoiceType
                .AnyAsync(it => it.InvoiceType == invoiceRent.InvoiceType);
            if (!invoiceTypeExists)
            {
                _logger.LogWarning("Invalid InvoiceTypeId {InvoiceTypeId}", invoiceRent.InvoiceType);
                return false;
            }

            var invoiceTypeId = await GetInvoiceTypeNameByIdAsync(invoiceRent.InvoiceType);

            decimal discountAmount = lease.MonthlyRent * (lease.Discount / 100m);
            decimal amountDue = lease.MonthlyRent - discountAmount;

            var previousInvoice = await _context.Set<InvoiceRental>()
                .Where(r => r.PropertyId == invoiceRent.PropertyId && r.Status != "Paid")
                .OrderByDescending(r => r.DueDate)
                .FirstOrDefaultAsync();

            decimal lateFee = 50;
            if (previousInvoice != null)
            {
                amountDue += previousInvoice.Amount + lateFee;
            }

            var newInvoice = new InvoiceRental
            {

                PropertyId = invoiceRent.PropertyId,
                Amount = amountDue,
                DueDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 5),
                RentMonth = (int)DateTime.UtcNow.Month,
                RentYear = (int)DateTime.UtcNow.Year,
                Status = "Pending",
                CreatedBy = "Web",
            };

            _context.Invoices.Add(newInvoice);
            var saved = await _context.SaveChangesAsync() > 0;

            _logger.LogInformation("Invoice created for TenantId {TenantId} with TotalAmountDue {Amount}",
                invoiceRent.PropertyId, newInvoice.Amount);

            return saved;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice for TenantId {TenantId}", invoiceRent.PropertyId);
            return false;
        }
    }

    public async Task<bool> UpdateInvoiceAsync(Invoice invoice)
    {
        try
        {
            _context.Invoices.Update(invoice);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating invoice ID {InvoiceId}", invoice.InvoiceId);
            return false;
        }
    }

    public async Task<bool> DeleteInvoiceAsync(int invoiceId)
    {
        try
        {
            var invoice = await GetInvoiceByIdAsync(invoiceId);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice ID {InvoiceId} not found for deletion", invoiceId);
                return false;
            }

            _context.Invoices.Remove(invoice);
            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting invoice ID {InvoiceId}", invoiceId);
            return false;
        }
    }

    public async Task<Lease?> GetLeaseInformationAsync(int propertyId)
    {
        try
        {
            return await _context.Leases
                .AsNoTracking()
                .Where(l => l.PropertyId == propertyId && l.IsActive == true)
                .OrderByDescending(l => l.StartDate)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lease for propertyId {propertyId}", propertyId);
            return null;
        }
    }

    public async Task<int?> GetInvoiceTypeNameByIdAsync(string invoiceTypeName)
    {
        try
        {
            return await _context.LkupInvoiceType
                .AsNoTracking()
                .Where(t => t.InvoiceType == invoiceTypeName)
                .Select(t => t.InvoiceTypeId)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve InvoiceTypeName for ID {InvoiceTypeId}", invoiceTypeName);
            return null;
        }
    }

    public Task<Invoice> GetMostRecentInvoiceByPropertyIdAsync(int propertyId)
    {
        throw new NotImplementedException();
    }
}