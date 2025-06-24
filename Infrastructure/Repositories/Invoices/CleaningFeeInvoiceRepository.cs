using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Application.Repositories.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class CleaningFeeInvoiceRepository : ICleaningFeeInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<CleaningFeeInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public CleaningFeeInvoiceRepository(MySqlDbContext context, ILogger<CleaningFeeInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> CreateCleaningFeeInvoiceAsync(CleaningFeeInvoiceCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("CleaningFeeInvoiceRepository is null.");
                return false;
            }

            try
            {
                int invoiceTypeId = await _invoiceRepository.InvoiceTypeExistsAsync(dto.InvoiceType);
                if (invoiceTypeId == -1)
                {
                    _logger.LogWarning("Invalid invoice type: {InvoiceType}", dto.InvoiceType);
                    return false;
                }

                int cleaningTypeId = await CleaningTypeExistsAsync(dto.CleaningType);
                if (cleaningTypeId == -1)
                {
                    _logger.LogWarning("Invalid cleaning type: {InvoiceType}", dto.CleaningType);
                    return false;
                }

                var CustomerName = await _invoiceRepository.GetPropertyOwnerNameAsync(dto.PropertyId);
                if (string.IsNullOrEmpty(CustomerName))
                {
                    _logger.LogWarning("No Customer Name found for PropertyId: {PropertyId}", dto.PropertyId);
                }
                
                var newInvoice = new CleaningFeeInvoice
                {
                    PropertyId = dto.PropertyId,
                    CustomerName = CustomerName ?? "Unknown",
                    InvoiceTypeId = invoiceTypeId,
                    InvoiceId = dto.InvoiceId,
                    CleaningTypeId = cleaningTypeId,
                    Amount = dto.Amount,
                    DueDate = dto.DueDate,
                    Notes = dto.Notes,
                    CreatedBy = "Web",
                    Status = "Pending"
                };

                _context.CleaningFeeInvoices.Add(newInvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                _logger.LogInformation("Cleaning Fee invoice created for PropertyId: {PropertyId}", dto.PropertyId);

                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Cleaning Fee invoice for PropertyId {PropertyId}", dto.PropertyId);
                return false;
            }
        }

        public async Task<CleaningFeeInvoice?> GetCleaningFeeInvoiceByIdAsync(int invoiceId)
        {
            return await _context.CleaningFeeInvoices.FindAsync(invoiceId);
        }

        public async Task<IEnumerable<CleaningFeeInvoice>> GetAllCleaningFeeInvoiceAsync()
        {
            return await _context.CleaningFeeInvoices.ToListAsync();
        }

        public async Task UpdateCleaningFeeInvoiceAsync(CleaningFeeInvoice invoice)
        {
            _context.CleaningFeeInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCleaningFeeInvoiceAsync(int invoiceId)
        {
            try
            {
                _context.Invoices.Remove(new Invoice { InvoiceId = invoiceId });
                var save = await _context.SaveChangesAsync();
                return save  >0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Cleaning Fee invoice with InvoiceId {InvoiceId}", invoiceId);
                return false;
            }
        }

        public async Task<int> CleaningTypeExistsAsync(string cleaningTypeName)
        {
            try
            {
                cleaningTypeName = cleaningTypeName.ToUpperInvariant();
                var cleaningType = await _context.LkupCleaningType
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.CleaningTypeName == cleaningTypeName);

                if (cleaningType == null)
                {
                    _logger.LogWarning("No active lease found for Cleaning type {cleaningTypeName}", cleaningTypeName);
                    return -1;
                }
                return cleaningType.CleaningTypeId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Cleaning type for cleaningTypeName {cleaningTypeName}", cleaningTypeName);
                return -1;
            }
        }
    }
}