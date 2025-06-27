using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Common.Helpers;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class LeaseTerminationInvoiceRepository : ILeaseTerminationInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<CleaningFeeInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public LeaseTerminationInvoiceRepository(MySqlDbContext context, ILogger<CleaningFeeInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> CreateLeaseTerminationInvoiceAsync(LeaseTerminationInvoiceCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("LeaseTerminationInvoiceCreateDto is null.");
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

                var newInvoice = new LeaseTerminationInvoice
                {
                    CustomerName = customerInvoiceInfo.CustomerName,
                    TenantId = customerInvoiceInfo.TenantId,
                    Email = customerInvoiceInfo.Email,
                    ReferenceNumber = referenceNumber,
                    Amount = amountDue,
                    DueDate = dto.DueDate,
                    PropertyId = dto.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    Notes = dto.Notes,
                    Status = "Pending",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "Web"                    
                };

                _context.LeaseTerminationInvoices.Add(newInvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                _logger.LogInformation("Lease Termination invoice created for PropertyId: {PropertyId}", dto.PropertyId);

                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Lease Terminatio invoice for PropertyId {PropertyId}", dto.PropertyId);
                return false;
            }
        }

        public async Task<LeaseTerminationInvoice?> GetLeaseTerminationInvoiceByIdAsync(int invoiceId)
        {
            return await _context.LeaseTerminationInvoices.FindAsync(invoiceId);
        }

        public async Task<IEnumerable<LeaseTerminationInvoice>> GetAllLeaseTerminationInvoiceAsync()
        {
            return await _context.LeaseTerminationInvoices.ToListAsync();
        }

        public async Task UpdateLeaseTerminationInvoiceAsync(LeaseTerminationInvoice invoice)
        {
            _context.LeaseTerminationInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteLeaseTerminationInvoiceAsync(int invoiceId)
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