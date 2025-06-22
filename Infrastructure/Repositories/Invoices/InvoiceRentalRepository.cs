using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class InvoiceRentalRepository : IInvoiceRentalRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<InvoiceRentalRepository> _logger;


        public InvoiceRentalRepository(MySqlDbContext context, ILogger<InvoiceRentalRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateInvoiceRentalAsync(InvoiceRental invoiceRent)
        {
            decimal lateFee = 50;
            _logger.LogInformation("Creating invoice for TenantId {TenantId}", invoiceRent.PropertyId);
            try
            {

                if (invoiceRent == null)
                {
                    _logger.LogWarning("InvoiceRent is null");
                    return false;
                }
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

                decimal discountAmount = lease.MonthlyRent * (lease.Discount / 100m);
                decimal amountDue = lease.MonthlyRent - discountAmount;

                var previousMonth = new DateTime(invoiceRent.DueDate.Year, invoiceRent.DueDate.Month, 1).AddMonths(-1);

                var previousInvoice = await _context.Set<Invoice>()
                    .Where(r =>
                        r.PropertyId == invoiceRent.PropertyId &&
                        r.Status != "Paid" &&
                        r.DueDate.Month == previousMonth.Month &&
                        r.DueDate.Year == previousMonth.Year)
                    .FirstOrDefaultAsync();

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
        public async Task<InvoiceRental?> GetInvoiceRentalByIdAsync(int invoiceId) =>
            await _context.InvoiceRentals
                          .Include(r => r.PropertyId) // optional: eager load relationships
                          .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

        public async Task<IEnumerable<InvoiceRental>> GetAllInvoiceRentalsAsync() =>
            await _context.InvoiceRentals.ToListAsync();

        public async Task<IEnumerable<InvoiceRental>> GetInvoiceRentalByMonthYearAsync(int month, int year) =>
            await _context.InvoiceRentals
                          .Where(i => i.RentMonth == month && i.RentYear == year)
                          .ToListAsync();

        public async Task UpdateAsync(InvoiceRental invoice)
        {
            _context.InvoiceRentals.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int invoiceId)
        {
            var invoice = await _context.InvoiceRentals.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.InvoiceRentals.Remove(invoice);
                await _context.SaveChangesAsync();
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
                return -1;
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
                    return -1;
                }

                return await GetInvoiceTypeNameByIdAsync(invoiceType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if invoice type exists for ID {invoiceType}", invoiceType);
                return -1;
            }
        }
    }
}