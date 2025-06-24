using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

public class PropertyTaxInvoiceRepository : IPropertyTaxInvoiceRepository
{
    private readonly MySqlDbContext _context;
    private readonly ILogger<PropertyTaxInvoiceRepository> _logger;
    private readonly IInvoiceRepository _invoiceRepository;

    public PropertyTaxInvoiceRepository(MySqlDbContext context, ILogger<PropertyTaxInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
    {
        _context = context;
        _logger = logger;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<bool> CreatePropertyTaxInvoiceAsync(PropertyTaxInvoiceCreateDto dto)
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

            decimal propertyTaxAmount = await _invoiceRepository.GetAmountDueAsync(dto, null);

            //override the Property Tax Amount if provided in the DTO
            if (dto.Amount > 0)
            {
                propertyTaxAmount = dto.Amount;
            }

            var CustomerName = await _invoiceRepository.GetPropertyOwnerNameAsync(dto.PropertyId);
            if (string.IsNullOrEmpty(CustomerName))
            {
                _logger.LogWarning("No Customer Name found for PropertyId: {PropertyId}", dto.PropertyId);
            }

            var newInvoice = new PropertyTaxInvoice
            {
                PropertyId = dto.PropertyId,
                CustomerName = CustomerName ?? "Unknown",
                InvoiceTypeId = invoiceTypeId,
                InvoiceId = dto.InvoiceId,
                Amount = propertyTaxAmount,
                DueDate = dto.DueDate,
                Notes = dto.Notes,
                TaxPeriodStart = dto.TaxPeriodStart,
                TaxPeriodEnd = dto.TaxPeriodEnd,
                CreatedBy = "Web",
                Status = "Pending"
            };

            _context.PropertyTaxInvoices.Add(newInvoice);
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

    public async Task<PropertyTaxInvoice?> GetPropertyTaxInvoiceByIdAsync(int invoiceId)
    {
        return await _context.PropertyTaxInvoices.FindAsync(invoiceId);
    }

    public async Task<IEnumerable<PropertyTaxInvoice>> GetAllPropertyTaxInvoiceAsync()
    {
        return await _context.PropertyTaxInvoices.ToListAsync();
    }

    public async Task UpdatePropertyTaxInvoiceAsync(PropertyTaxInvoice invoice)
    {
        _context.PropertyTaxInvoices.Update(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeletePropertyTaxInvoiceAsync(int invoiceId)
    {
        try
        {
            _context.Invoices.Remove(new Invoice { InvoiceId = invoiceId });
            var save = await _context.SaveChangesAsync();
            return save > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting property tax invoice with InvoiceId {InvoiceId}", invoiceId);
            return false;
        }
    }
}