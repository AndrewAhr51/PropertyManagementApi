using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Common.Helpers;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class RentInvoiceRepository : IRentInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<RentInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public RentInvoiceRepository(MySqlDbContext context, ILogger<RentInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
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
                
                var customerInvoiceInfo = await _invoiceRepository.GetPropertyTenantInfoAsync(dto.PropertyId);
                if (customerInvoiceInfo == null)
                {
                    _logger.LogWarning("No tenant information found for PropertyId {PropertyId}", dto.PropertyId);
                    return false;
                }

                var amountDueTask = _invoiceRepository.GetAmountDueAsync(dto, null);
                decimal amountDue = await amountDueTask;

                if (amountDue == 0)
                {
                    _logger.LogWarning("No Rental amoount information found for PropertyId {PropertyId}", dto.PropertyId);
                    return false;
                }
                else {
                    _logger.LogInformation("Amount due for TenantId {TenantId} is {AmountDue}", dto.PropertyId, amountDue);
                }

                //Override the amount due with late fee if applicable
                if (dto.Amount > 0)
                {
                    amountDue = dto.Amount;
                }

                var referenceNumber = ReferenceNumberHelper.Generate("REF", dto.PropertyId);

                var newInvoice = new RentInvoice
                {
                    CustomerName = customerInvoiceInfo.CustomerName,
                    TenantId = customerInvoiceInfo.TenantId,
                    Email = customerInvoiceInfo.Email,
                    ReferenceNumber = referenceNumber,
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
                          .Include(r => r.Property) // optional: eager load relationships
                          .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

        public async Task<IEnumerable<RentInvoice>> GetAllInvoiceRentalsAsync() =>
            await _context.RentInvoices.ToListAsync();

        public async Task<IEnumerable<RentInvoice>> GetInvoiceRentalByMonthYearAsync(int month, int year) =>
            await _context.RentInvoices
                          .Where(i => i.RentMonth == month && i.RentYear == year)
                          .ToListAsync();

        public async Task<bool> UpdateInvoiceRentalAsync(RentInvoiceCreateDto rentInvoice)
        {
            if (rentInvoice == null)
            {
                _logger.LogWarning("Attempted to update a null RentInvoice");
                return false;
            }
            var existingInvoice = await _context.RentInvoices.FindAsync(rentInvoice.InvoiceId);
            if (existingInvoice == null)
            {
                _logger.LogWarning("No RentInvoice found with InvoiceId {InvoiceId}", rentInvoice.InvoiceId);
                return false;
            }
            // Update the properties of the existing invoice
            existingInvoice.Amount = rentInvoice.Amount;
            existingInvoice.DueDate = rentInvoice.DueDate;
            existingInvoice.RentMonth = rentInvoice.RentMonth;
            existingInvoice.RentYear = rentInvoice.RentYear;
            existingInvoice.Notes = rentInvoice.Notes;
            existingInvoice.PropertyId = rentInvoice.PropertyId;
            existingInvoice.CreatedDate = DateTime.UtcNow;

            _logger.LogInformation("Updating RentInvoice with InvoiceId {InvoiceId}", rentInvoice.InvoiceId);

            _context.Invoices.Update(existingInvoice);
            var save = await _context.SaveChangesAsync();

            return save > 0;
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