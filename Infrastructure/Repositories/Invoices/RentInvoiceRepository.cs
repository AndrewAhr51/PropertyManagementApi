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

        public async Task<bool> CreateInvoiceRentalAsync(RentInvoiceCreateDto dto)
        {
            decimal lateFee = 50;
            _logger.LogInformation("Creating invoice for TenantId {TenantId}", dto.PropertyId);
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("InvoiceRent is null");
                    return false;
                }

                var invoiceTypeId = await _invoiceRepository.InvoiceTypeExistsAsync(dto.InvoiceType);
                if (invoiceTypeId == null)
                {
                    throw new ArgumentException($"Invalid invoice type: {dto.InvoiceType}");
                }

                var amountDueTask = _invoiceRepository.GetAmountDueAsync(dto, null);
                decimal amountDue = await amountDueTask;

                if (amountDue == 0)
                {
                    throw new ArgumentException($"Error retrieving the Amount due information from the lease Property: {dto.PropertyId}");
                }

                var CustomerName = await _invoiceRepository.GetPropertyOwnerNameAsync(dto.PropertyId);
                if (string.IsNullOrEmpty(CustomerName))
                {
                    _logger.LogWarning("No Customer Name found for PropertyId: {PropertyId}", dto.PropertyId);
                }

                //Override the amount due with late fee if applicable
                if (dto.Amount > 0)
                {
                    amountDue = dto.Amount;
                }

                var newInvoice = new RentInvoice
                {
                    InvoiceId = dto.InvoiceId,
                    CustomerName = CustomerName ?? "Unknown",
                    Amount = amountDue,
                    DueDate = dto.DueDate,
                    PropertyId = dto.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    RentMonth = dto.RentMonth,
                    RentYear = dto.RentYear,
                    Notes = dto.Notes,
                    CreatedDate = DateTime.UtcNow
                };
                               
                _context.Invoices.Add(newInvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                _logger.LogInformation("Invoice created for TenantId {TenantId} with TotalAmountDue {Amount}",
                    dto.PropertyId, newInvoice.Amount);

                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice for TenantId {TenantId}", dto.PropertyId);
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

        public async Task UpdateInvoiceRentalAsync(RentInvoice invoice)
        {
            _context.RentInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteInvoiceRentalAsync(int invoiceId)
        {
            try
            {
                _context.Invoices.Remove(new Invoice { InvoiceId = invoiceId });
                var save = await _context.SaveChangesAsync();
                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Lease Termination Invoice with InvoiceId {InvoiceId}", invoiceId);
                return false;
            }
        }

    }
}