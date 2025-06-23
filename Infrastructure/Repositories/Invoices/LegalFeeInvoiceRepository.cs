using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;

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
                int invoiceTypeId = await _invoiceRepository.InvoiceTypeExistsAsync(dto.InvoiceType);
                if (invoiceTypeId == -1)
                {
                    _logger.LogWarning("Invalid invoice type: {InvoiceType}", dto.InvoiceType);
                    return false;
                }

                var newInvoice = new LegalFeeInvoice
                {
                    PropertyId = dto.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    InvoiceId = dto.InvoiceId,
                    CaseReference = dto.CaseReference,
                    LawFirm = dto.LawFirm,
                    Amount = dto.Amount,
                    CreatedBy = "Web",
                    Status = "Pending"
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
