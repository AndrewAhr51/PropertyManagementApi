using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class RentInvoiceRepository : IRentInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<RentInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public RentInvoiceRepository(MySqlDbContext context,  ILogger<RentInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> CreateInvoiceRentalAsync(RentInvoiceCreateDto rentInvoiceCreate)
        {
            decimal lateFee = 50;
            _logger.LogInformation("Creating invoice for TenantId {TenantId}", rentInvoiceCreate.PropertyId);
            try
            {
                if (rentInvoiceCreate == null)
                {
                    _logger.LogWarning("InvoiceRent is null");
                    return false;
                }

                var invoiceTypeId = await _invoiceRepository.InvoiceTypeExistsAsync(rentInvoiceCreate.InvoiceType);
                if (invoiceTypeId == null)
                {
                    throw new ArgumentException($"Invalid invoice type: {rentInvoiceCreate.InvoiceType}");
                }

                var amountDueTask = _invoiceRepository.GetAmountDueAsync(rentInvoiceCreate, invoiceTypeId);
                decimal amountDue = await amountDueTask;

                if (amountDue == 0)
                {
                    throw new ArgumentException($"Error retrieving the Amount due information from the lease Property: {rentInvoiceCreate.PropertyId}");
                }

                var newInvoice = new RentInvoice
                {
                    InvoiceId = rentInvoiceCreate.InvoiceId,
                    Amount = rentInvoiceCreate.Amount,
                    DueDate = rentInvoiceCreate.DueDate,
                    PropertyId = rentInvoiceCreate.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    RentMonth = rentInvoiceCreate.RentMonth,
                    RentYear = rentInvoiceCreate.RentYear,
                    Notes = rentInvoiceCreate.Notes,
                    CreatedDate = DateTime.UtcNow
                };
                               
                _context.Invoices.Add(newInvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                _logger.LogInformation("Invoice created for TenantId {TenantId} with TotalAmountDue {Amount}",
                    rentInvoiceCreate.PropertyId, newInvoice.Amount);

                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice for TenantId {TenantId}", rentInvoiceCreate.PropertyId);
                return false;
            }
        }
        public async Task<RentInvoice?> GetInvoiceRentalByIdAsync(int invoiceId) =>
            await _context.RentInvoices
                          .Include(r => r.PropertyId) // optional: eager load relationships
                          .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

        public async Task<IEnumerable<RentInvoice>> GetAllInvoiceRentalsAsync() =>
            await _context.RentInvoices.ToListAsync();

        public async Task<IEnumerable<RentInvoice>> GetInvoiceRentalByMonthYearAsync(int month, int year) =>
            await _context.RentInvoices
                          .Where(i => i.RentMonth == month && i.RentYear == year)
                          .ToListAsync();

        public async Task UpdateAsync(RentInvoice invoice)
        {
            _context.RentInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int invoiceId)
        {
            var invoice = await _context.RentInvoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.RentInvoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

    }
}