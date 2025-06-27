using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Common.Helpers;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class SecurityDepositInvoiceRepository : ISecurityDepositInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<RentInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public SecurityDepositInvoiceRepository(MySqlDbContext context, ILogger<RentInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> CreateSecurityDepositInvoiceAsync(SecurityDepositInvoiceCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("SecurityDepositInvoiceCreateDto is null.");
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
                    _logger.LogWarning("No Security Deposit information found for PropertyId {PropertyId}", dto.PropertyId);
                    return false;
                }
                else
                {
                    _logger.LogInformation("Security Deposit amount due for TenantId {TenantId} is {AmountDue}", dto.PropertyId, amountDue);
                }

                //Override the amount due with late fee if applicable
                if (dto.Amount > 0)
                {
                    amountDue = dto.Amount;
                }

                var referenceNumber = ReferenceNumberHelper.Generate("REF", dto.PropertyId);

                var newInvoice = new SecurityDepositInvoice
                {
                    CustomerName = customerInvoiceInfo.CustomerName,
                    TenantId = customerInvoiceInfo.TenantId,
                    Email = customerInvoiceInfo.Email,
                    ReferenceNumber = referenceNumber,
                    Amount = (decimal)amountDue,
                    DueDate = dto.DueDate,
                    PropertyId = dto.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    Notes = dto.Notes,
                    DepositAmount = (decimal)amountDue,
                    IsRefundable = dto.IsRefundable,
                    Status = "Pending",
                    CreatedBy = "Web",
                    CreatedDate = DateTime.UtcNow
                };

                _context.SecurityDepositInvoices.Add(newInvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                _logger.LogInformation("Security deposit invoice created for PropertyId {PropertyId} with amount {Amount}",
                    dto.PropertyId, amountDue);

                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating security deposit invoice for PropertyId {PropertyId}", dto.PropertyId);
                return false;
            }
        }

        public async Task<SecurityDepositInvoice?> GetSecurityDepositInvoiceByIdAsync(int invoiceId)
        {
            return await _context.SecurityDepositInvoices.FindAsync(invoiceId);
        }

        public async Task<IEnumerable<SecurityDepositInvoice>> GetAllSecurityDepositInvoiceAsync()
        {
            return await _context.SecurityDepositInvoices.ToListAsync();
        }

        public async Task UpdateSecurityDepositInvoiceAsync(SecurityDepositInvoice invoice)
        {
            _context.SecurityDepositInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteSecurityDepositInvoiceAsync(int invoiceId)
        {
            try
            {
                _context.Invoices.Remove(new Invoice { InvoiceId = invoiceId });
                var save = await _context.SaveChangesAsync();
                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting security deposit invoice with InvoiceId {InvoiceId}", invoiceId);
                return false;
            }
        }

        public async Task<decimal?> SecurityDepositAmountAsync(int propertyId)
        {
            try
            {
                var lease = await _context.Leases
                    .AsNoTracking()
                    .Where(l => l.PropertyId == propertyId && l.IsActive == true)
                    .OrderByDescending(l => l.StartDate)
                    .FirstOrDefaultAsync();
                if (lease == null)
                {
                    _logger.LogWarning("No active lease found for PropertyId {PropertyId}", propertyId);
                    return -1;
                }
                return lease.DepositAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving security deposit amount for PropertyId {PropertyId}", propertyId);
                return -1;
            }
        }
                   
    }
}