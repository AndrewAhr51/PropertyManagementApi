using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Common.Helpers;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class ParkingFeeInvoiceRepository : IParkingFeeInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<ParkingFeeInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public ParkingFeeInvoiceRepository(MySqlDbContext context, ILogger<ParkingFeeInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> CreateParkingFeeInvoiceAsync(ParkingFeeInvoiceCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("CreateParkingFeeInvoiceAsync is null.");
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
                    _logger.LogWarning("No Parking fee amount information found for PropertyId {PropertyId}", dto.PropertyId);
                    return false;
                }
                else
                {
                    _logger.LogInformation("Parking fee amount due for TenantId {TenantId} is {AmountDue}", dto.PropertyId, amountDue);
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
                    Amount = amountDue,
                    DueDate = dto.DueDate,
                    PropertyId = dto.PropertyId,
                    InvoiceTypeId = invoiceTypeId,
                    Notes = dto.Notes,
                    Status = "Pending",
                    CreatedBy = "Web",
                    CreatedDate = DateTime.UtcNow
                };

                _context.SecurityDepositInvoices.Add(newInvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                _logger.LogInformation("Parking fee invoice created for PropertyId {PropertyId} with amount {Amount}",
                    dto.PropertyId, dto.Amount);

                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating parking fee invoice for PropertyId {PropertyId}", dto.PropertyId);
                return false;
            }
        }

        public async Task<ParkingFeeInvoice?> GetParkingFeeInvoiceByIdAsync(int invoiceId)
        {
            return await _context.ParkingFeeInvoices.FindAsync(invoiceId);
        }

        public async Task<IEnumerable<ParkingFeeInvoice>> GetAllParkingFeeInvoiceAsync()
        {
            return await _context.ParkingFeeInvoices.ToListAsync();
        }

        public async Task UpdateParkingFeeInvoiceAsync(ParkingFeeInvoice invoice)
        {
            _context.ParkingFeeInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteParkingFeeInvoiceAsync(int invoiceId)
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