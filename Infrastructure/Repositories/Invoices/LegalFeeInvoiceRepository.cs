using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Common.Helpers;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class LegalFeeInvoiceRepository : ILegalFeeInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<LegalFeeInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public LegalFeeInvoiceRepository(MySqlDbContext context, ILogger<LegalFeeInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> CreateLegalFeeInvoiceAsync(LegalFeeInvoiceCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("CreateLegalFeeInvoiceAsync is null.");
                return false;
            }

            try
            {
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
                else
                {
                    _logger.LogInformation("Amount due for TenantId {TenantId} is {AmountDue}", dto.PropertyId, amountDue);
                }

                //Override the amount due with late fee if applicable
                if (dto.Amount > 0)
                {
                    amountDue = dto.Amount;
                }


                var referenceNumber = ReferenceNumberHelper.Generate("REF", dto.PropertyId);

                var newInvoice = new LegalFeeInvoice
                {
                    CustomerName = customerInvoiceInfo.CustomerName,
                    TenantId = customerInvoiceInfo.TenantId,
                    Email = customerInvoiceInfo.Email,
                    ReferenceNumber = referenceNumber,
                    CaseReference = dto.CaseReference,
                    Amount = amountDue,
                    DueDate = dto.DueDate,
                    PropertyId = dto.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    Notes = dto.Notes,
                    LawFirm = dto.LawFirm,
                    Status = "Pending",
                    CreatedBy = "Web",
                    CreatedDate = DateTime.UtcNow
                };

                _context.LegalFeeInvoices.Add(newInvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                _logger.LogInformation("Legal fee invoice created for PropertyId {PropertyId} with amount {Amount}",
                    dto.PropertyId, dto.Amount);

                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Legal fee invoice for PropertyId {PropertyId}", dto.PropertyId);
                return false;
            }
        }

        public async Task<LegalFeeInvoice?> GetLegalFeeInvoiceByIdAsync(int invoiceId)
        {
            return await _context.LegalFeeInvoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.InvoiceId == invoiceId);
        }

        public async Task<IEnumerable<LegalFeeInvoice>> GetAllLegalFeeInvoiceAsync()
        {
            return await _context.LegalFeeInvoices.AsNoTracking().ToListAsync();
        }

        public async Task<bool> UpdateLegalFeeInvoiceAsync(LegalFeeInvoiceCreateDto dto)
        {
            var updatedInvoice =  await _context.LegalFeeInvoices
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.InvoiceId == dto.InvoiceId);

            if (updatedInvoice == null)
            {
                _logger.LogWarning("LegalFeeInvoice with InvoiceId {InvoiceId} not found for update.", dto.InvoiceId);
                return false;
            }

            updatedInvoice.CaseReference = dto.CaseReference;
            updatedInvoice.LawFirm = dto.LawFirm;
            updatedInvoice.Amount = dto.Amount;
            updatedInvoice.DueDate = dto.DueDate;
            updatedInvoice.Notes = dto.Notes;
            updatedInvoice.CreatedBy = "Web";
            
            _context.LegalFeeInvoices.Update(updatedInvoice);
            var save = await _context.SaveChangesAsync();

            return save > 0;
        }

        public async Task<bool> DeleteLegalFeeInvoiceAsync(int invoiceId)
        {
            try
            {
                _context.Invoices.Remove(new Invoice { InvoiceId = invoiceId });
                var save = await _context.SaveChangesAsync();
                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Parking Fee Invoice with InvoiceId {InvoiceId}", invoiceId);
                return false;
            }
        }
    }
}
