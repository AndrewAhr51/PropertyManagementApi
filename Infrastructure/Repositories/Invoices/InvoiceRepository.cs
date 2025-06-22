using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<RentInvoiceRepository> _logger;

        public InvoiceRepository(MySqlDbContext context, ILogger<RentInvoiceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<decimal> GetAmountDueAsync(RentInvoiceCreateDto invoice, int invoiceTypeId)
        {
            try
            {
                if (invoice == null)
                {
                    _logger.LogWarning("Invoice is null");
                    return 0;
                }

                if (!await _context.Property.AnyAsync(t => t.PropertyId == invoice.PropertyId))
                {
                    _logger.LogWarning("PropertyId {PropertyId} not found", invoice.PropertyId);
                    return 0;
                }

                var lease = await GetLeaseInformationAsync(invoice.PropertyId);
                if (lease == null)
                {
                    _logger.LogWarning("No active lease found for PropertyId {PropertyId}", invoice.PropertyId);
                    return 0;
                }

                decimal discountAmount = lease.MonthlyRent * (lease.Discount / 100m);
                decimal amountDue = lease.MonthlyRent - discountAmount;

                var previousMonth = new DateTime(invoice.DueDate.Year, invoice.DueDate.Month, 1).AddMonths(-1);

                var previousInvoice = await _context.RentInvoices
                    .Where(r =>
                        r.InvoiceTypeId == invoiceTypeId &&
                        r.PropertyId == invoice.PropertyId &&
                        r.Status != "Paid" &&
                        r.RentMonth == previousMonth.Month &&
                        r.RentYear == previousMonth.Year)
                    .FirstOrDefaultAsync();

                if (previousInvoice != null)
                {
                    amountDue += previousInvoice.Amount;
                }

                return amountDue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice for PropertyId {PropertyId}", invoice.PropertyId);
                return 0;
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
                return null; // Fix for CS8603: Return null explicitly for nullable type.
            }
        }

        public async Task<int> GetInvoiceTypeNameByIdAsync(string invoiceTypeName)
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
                return -1; // Fix for CS8603: Return a default value for non-nullable type.
            }
        }

        public async Task<int> InvoiceTypeExistsAsync(string invoiceType)
        {
            try
            {
                var invoiceTypeExists = await _context.LkupInvoiceType
                    .AnyAsync(it => it.InvoiceType == invoiceType);
                if (!invoiceTypeExists)
                {
                    _logger.LogWarning("Invalid InvoiceTypeId {InvoiceTypeId}", invoiceType);
                    return -1; // Fix for CS8603: Return a default value for non-nullable type.
                }

                return await GetInvoiceTypeNameByIdAsync(invoiceType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if invoice type exists for ID {invoiceType}", invoiceType);
                return -1; // Fix for CS8603: Return a default value for non-nullable type.
            }
        }
    }
}
