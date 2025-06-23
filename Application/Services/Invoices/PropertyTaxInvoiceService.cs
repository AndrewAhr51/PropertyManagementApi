using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

public class PropertyTaxInvoiceService : IPropertyTaxInvoiceService
{
    private readonly IPropertyTaxInvoiceRepository _repository;

    public PropertyTaxInvoiceService(IPropertyTaxInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreatePropertyTaxInvoiceAsync(PropertyTaxInvoiceCreateDto dto)
    {
        var save = await _repository.CreatePropertyTaxInvoiceAsync(dto);
        return save;
    }

    public Task<PropertyTaxInvoice?> GetByPropertyTaxInvoiceIdAsync(int invoiceId) =>
        _repository.GetPropertyTaxInvoiceByIdAsync(invoiceId);

    public Task<IEnumerable<PropertyTaxInvoice>> GetAllPropertyTaxInvoiceAsync() =>
        _repository.GetAllPropertyTaxInvoiceAsync();

    public async Task<bool> UpdatePropertyTaxInvoiceAsync(PropertyTaxInvoiceCreateDto dto)
    {
        var existing = await _repository.GetPropertyTaxInvoiceByIdAsync(dto.InvoiceId);
        if (existing is null) return false;

        existing.TaxPeriodStart = dto.TaxPeriodStart;
        existing.TaxPeriodEnd = dto.TaxPeriodEnd;

        await _repository.UpdatePropertyTaxInvoiceAsync(existing);
        return true;
    }

    public async Task<bool> DeletePropertyTaxInvoiceAsync(int invoiceId)
    {
        var save = await _repository.DeletePropertyTaxInvoiceAsync(invoiceId);
        return save;
    }
}