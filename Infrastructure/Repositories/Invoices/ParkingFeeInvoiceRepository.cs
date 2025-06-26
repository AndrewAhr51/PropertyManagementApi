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
                int invoiceTypeId = await _invoiceRepository.InvoiceTypeExistsAsync(dto.InvoiceType);
                if (invoiceTypeId == -1)
                {
                    _logger.LogWarning("Invalid invoice type: {InvoiceType}", dto.InvoiceType);
                    return false;
                }
                var CustomerName = await _invoiceRepository.GetPropertyOwnerNameAsync(dto.PropertyId);
                
                if (string.IsNullOrEmpty(CustomerName))
                {
                    _logger.LogWarning("No Customer Name found for PropertyId: {PropertyId}", dto.PropertyId);
                }
                var newInvoice = new SecurityDepositInvoice
                {
                    PropertyId = dto.PropertyId,
                    ReferenceNumber = ReferenceNumberHelper.Generate("INV", dto.PropertyId),
                    CustomerName = CustomerName ?? "Unknown",
                    InvoiceTypeId = invoiceTypeId,
                    DueDate = dto.DueDate,
                    Amount = dto.Amount,
                    CreatedBy = "Web",
                    Status = "Pending"
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