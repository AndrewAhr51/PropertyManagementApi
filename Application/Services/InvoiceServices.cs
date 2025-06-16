using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<InvoiceDto> GetInvoiceByIdAsync(int invoiceId)
    {
        var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
        return invoice != null ? MapToDto(invoice) : null;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
    {
        var invoices = await _invoiceRepository.GetAllInvoicesAsync();
        return invoices.Select(MapToDto).ToList();
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoicesByTenantIdAsync(int tenantId)
    {
        var invoices = await _invoiceRepository.GetInvoicesByTenantIdAsync(tenantId);
        return invoices.Select(MapToDto).ToList();
    }

    public async Task<bool> CreateInvoiceAsync(InvoiceDto invoiceDto)
    {
        var invoice = MapToEntity(invoiceDto);
        return await _invoiceRepository.AddInvoiceAsync(invoice);
    }

    public async Task<bool> UpdateInvoiceAsync(InvoiceDto invoiceDto)
    {
        var invoice = MapToEntity(invoiceDto);
        return await _invoiceRepository.UpdateInvoiceAsync(invoice);
    }

    public async Task<bool> DeleteInvoiceAsync(int invoiceId) =>
        await _invoiceRepository.DeleteInvoiceAsync(invoiceId);

    private InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            InvoiceId = invoice.InvoiceId,
            TenantId = invoice.TenantId,
            PropertyId = invoice.PropertyId,
            AmountDue = invoice.AmountDue,
            DueDate = invoice.DueDate,
            BillingPeriod = invoice.BillingPeriod,
            BillingMonth = invoice.BillingMonth,
            LateFee = invoice.LateFee,
            DiscountsApplied = invoice.DiscountsApplied,
            IsPaid = invoice.IsPaid,
            PaymentDate = invoice.PaymentDate,
            PaymentMethod = invoice.PaymentMethod,
            PaymentReference = invoice.PaymentReference,
            InvoiceStatus = invoice.InvoiceStatus,
            InvoiceTypeId = invoice.InvoiceTypeId,
            GeneratedBy = invoice.GeneratedBy,
            Notes = invoice.Notes,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt
        };
    }

    private Invoice MapToEntity(InvoiceDto dto)
    {
        return new Invoice
        {
            InvoiceId = dto.InvoiceId,
            TenantId = dto.TenantId,
            PropertyId = dto.PropertyId,
            AmountDue = dto.AmountDue,
            DueDate = dto.DueDate,
            BillingPeriod = dto.BillingPeriod,
            BillingMonth = dto.BillingMonth,
            LateFee = dto.LateFee,
            DiscountsApplied = dto.DiscountsApplied,
            IsPaid = dto.IsPaid,
            PaymentDate = dto.PaymentDate,
            PaymentMethod = dto.PaymentMethod,
            PaymentReference = dto.PaymentReference,
            InvoiceStatus = dto.InvoiceStatus,
            InvoiceTypeId = dto.InvoiceTypeId,
            GeneratedBy = dto.GeneratedBy,
            Notes = dto.Notes,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
    }
}