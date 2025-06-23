using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

public class InsuranceInvoiceRepository : IInsuranceInvoiceRepository
{
    private readonly MySqlDbContext _context;
    private readonly ILogger<InsuranceInvoiceRepository> _logger;
    private readonly IInvoiceRepository _invoiceRepository;

    public InsuranceInvoiceRepository(MySqlDbContext context, ILogger<InsuranceInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
    {
        _context = context;
        _logger = logger;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<bool> CreateInsuranceInvoiceAsync(InsuranceInvoiceCreateDto dto)
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

            var amountDueTask = _invoiceRepository.GetAmountDueAsync(dto, null);
            decimal amountDue = await amountDueTask;

            if (amountDue == 0)
            {
                throw new ArgumentException($"Error retrieving the Amount due information from the insurance for Property: {dto.PropertyId}");
            }

            var newInvoice = new InsuranceInvoice
            {
                PropertyId = dto.PropertyId,
                InvoiceTypeId = invoiceTypeId,
                InvoiceId = dto.InvoiceId,
                CoveragePeriodEnd = dto.CoveragePeriodEnd,
                CoveragePeriodStart = dto.CoveragePeriodStart,
                PolicyNumber = dto.PolicyNumber,
                Amount = dto.Amount,
                DueDate = dto.DueDate,
                Notes = dto.Notes,
                CreatedBy = "Web",
                Status = "Pending"
            };

            _context.InsuranceInvoices.Add(newInvoice);
            var saved = await _context.SaveChangesAsync() > 0;

            _logger.LogInformation("Insurance invoice created for PropertyId: {PropertyId}", dto.PropertyId);

            return saved;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Insurance invoice for PropertyId {PropertyId}", dto.PropertyId);
            return false;
        }
    }

    public async Task<InsuranceInvoice?> GetInsuranceInvoiceByIdAsync(int invoiceId)
    {
        return await _context.InsuranceInvoices.FindAsync(invoiceId);
    }

    public async Task<IEnumerable<InsuranceInvoice>> GetAllInsuranceInvoiceAsync()
    {
        return await _context.InsuranceInvoices.ToListAsync();
    }

    public async Task UpdateInsuranceInvoiceAsync(InsuranceInvoice invoice)
    {
        _context.InsuranceInvoices.Update(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteInsuranceInvoiceAsync(int invoiceId)
    {
        try
        {
            _context.Invoices.Remove(new Invoice { InvoiceId = invoiceId });
            var save = await _context.SaveChangesAsync();
            return save > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting insurance invoice with InvoiceId {InvoiceId}", invoiceId);
            return false;
        }
    }
}