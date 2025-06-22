using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Application.Repositories.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;

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
                int invoiceTypeId = await _invoiceRepository.InvoiceTypeExistsAsync(dto.InvoiceType);
                if (invoiceTypeId == -1)
                {
                    _logger.LogWarning("Invalid invoice type: {InvoiceType}", dto.InvoiceType);
                    return false;
                }

                var depositAmount = await SecurityDepositAmountAsync(dto.PropertyId);
                if (depositAmount == -1)
                {
                    _logger.LogWarning("Invalid security deposit amount for PropertyId {PropertyId}", dto.PropertyId);
                    return false;
                }

                var newInvoice = new SecurityDepositInvoice
                {
                    PropertyId = dto.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    InvoiceId = dto.InvoiceId,
                    DueDate = dto.DueDate,
                    DepositAmount = (decimal)depositAmount,
                    IsRefundable = dto.IsRefundable,
                    Notes = dto.Notes,
                    CreatedBy = "Web",
                    Status = "Pending"
                };

                _context.SecurityDepositInvoices.Add(newInvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                _logger.LogInformation("Security deposit invoice created for PropertyId {PropertyId} with amount {Amount}",
                    dto.PropertyId, depositAmount);

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

        public async Task DeleteSecurityDepositInvoiceAsync(int invoiceId)
        {
            var invoice = await _context.SecurityDepositInvoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.SecurityDepositInvoices.Remove(invoice);
                await _context.SaveChangesAsync();
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