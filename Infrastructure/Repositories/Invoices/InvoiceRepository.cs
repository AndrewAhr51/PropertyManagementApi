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
        private readonly ILogger<InvoiceDto> _logger;

        public InvoiceRepository(MySqlDbContext context, ILogger<InvoiceDto> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<decimal> GetAmountDueAsync(InvoiceDto invoice, string? UtilityType)
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

                var previousMonth = new DateTime(invoice.DueDate.Year, invoice.DueDate.Month, 1).AddMonths(-1);
                decimal amountDue = 0;
                Invoice? previousInvoice = null;

                switch (invoice.InvoiceType)
                {
                    case "Rent":
                        {
                            int invoiceTypeId = await GetInvoiceTypeNameByIdAsync("Rent");

                            var lease = await GetLeaseInformationAsync(invoice.PropertyId);
                            if (lease == null)
                            {
                                _logger.LogWarning("No active lease found for PropertyId {PropertyId}", invoice.PropertyId);
                                return 0;
                            }

                            decimal discount = lease.MonthlyRent * (lease.Discount / 100m);
                            amountDue = lease.MonthlyRent - discount;

                            previousInvoice = await _context.RentInvoices
                                .Where(r => r.InvoiceTypeId == invoiceTypeId &&
                                            r.PropertyId == invoice.PropertyId &&
                                            r.Status != "Paid" &&
                                            r.DueDate.Month == previousMonth.Month &&
                                            r.DueDate.Year == previousMonth.Year)
                                .FirstOrDefaultAsync();
                            break;
                        }

                    case "Utilities":
                        {
                            int invoiceTypeId = await GetInvoiceTypeNameByIdAsync("Utilities");
                            int utilityTypeId = await GetUtilityTypeNameByIdAsync(UtilityType);

                            previousInvoice = await _context.UtilityInvoices
                                .Where(r => r.InvoiceTypeId == invoiceTypeId &&
                                            r.UtilityTypeId == utilityTypeId &&
                                            r.PropertyId == invoice.PropertyId &&
                                            r.Status != "Paid" &&
                                            r.DueDate.Month == previousMonth.Month &&
                                            r.DueDate.Year == previousMonth.Year)
                                .FirstOrDefaultAsync();
                            break;
                        }

                    default:
                        _logger.LogWarning("Invalid InvoiceType {InvoiceType}", invoice.InvoiceType);
                        return 0;
                }

                if (previousInvoice != null)
                {
                    amountDue += previousInvoice.Amount;
                }

                return amountDue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating amount due for PropertyId {PropertyId}", invoice?.PropertyId);
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

        public async Task<int> GetUtilityTypeNameByIdAsync(string utilityType)
        {
            try
            {
                return await _context.LkupUtilities
                    .AsNoTracking()
                    .Where(t => t.UtilityName == utilityType)
                    .Select(t => t.UtilityId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve utilityType for {utilityType}", utilityType);
                return -1; // Fix for CS8603: Return a default value for non-nullable type.
            }
        }
    }
}
