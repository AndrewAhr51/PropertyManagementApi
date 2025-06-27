using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Invoice;

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
                var invoiceTypeId = await _invoiceRepository.InvoiceTypeExistsAsync(dto.InvoiceType);
                if (invoiceTypeId == null)
                {
                    throw new ArgumentException($"Invalid invoice type: {dto.InvoiceType}");
                }

                int cleaningTypeId = await CleaningTypeExistsAsync(dto.CleaningType);
                if (cleaningTypeId == -1)
                {
                    _logger.LogWarning("Invalid cleaning type: {InvoiceType}", dto.CleaningType);
                    return false;
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
                    _logger.LogWarning("No Cleaning fee amount information found for PropertyId {PropertyId}", dto.PropertyId);
                }
                else
                {
                    dto.Amount += amountDue;
                    _logger.LogInformation("Cleaning fee amount due for TenantId {TenantId} is {AmountDue}", dto.PropertyId, amountDue);
                }

                //Override the amount due with late fee if applicable
                if (dto.Amount > 0)
                {
                    amountDue = dto.Amount;
                }

                var referenceNumber = ReferenceNumberHelper.Generate("REF", dto.PropertyId);

                var newInvoice = new CleaningFeeInvoice
                {
                    CustomerName = customerInvoiceInfo.CustomerName,
                    TenantId = customerInvoiceInfo.TenantId,
                    Email = customerInvoiceInfo.Email,
                    ReferenceNumber = referenceNumber,
                    Amount = dto.Amount,
                    DueDate = dto.DueDate,
                    PropertyId = dto.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    CleaningTypeId = cleaningTypeId,
                    Notes = dto.Notes,
                    CreatedDate = DateTime.UtcNow,
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