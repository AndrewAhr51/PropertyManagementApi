using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

public class InsuranceInvoiceService : IInsuranceInvoiceService
{
    private readonly IInsuranceInvoiceRepository _repository;

    public InsuranceInvoiceService(IInsuranceInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreateInsuranceInvoiceAsync(InsuranceInvoiceCreateDto dto)
    {
        var save = await _repository.CreateInsuranceInvoiceAsync(dto);
        return save;
    }

    public Task<InsuranceInvoice?> GetInsuranceInvoiceByIdAsync(int invoiceId) =>
        _repository.GetInsuranceInvoiceByIdAsync(invoiceId);

    public Task<IEnumerable<InsuranceInvoice>> GetAllInsuranceInvoiceAsync() =>
        _repository.GetAllInsuranceInvoiceAsync();

    public async Task<bool> UpdateInsuranceInvoiceAsync(InsuranceInvoiceCreateDto dto)
    {
        var existing = await _repository.GetInsuranceInvoiceByIdAsync(dto.InvoiceId);
        if (existing is null) return false;

        existing.PolicyNumber = dto.PolicyNumber;
        existing.CoveragePeriodStart = dto.CoveragePeriodStart;
        existing.CoveragePeriodEnd = dto.CoveragePeriodEnd;

        await _repository.UpdateInsuranceInvoiceAsync(existing);
        return true;
    }

    public async Task<bool> DeleteInsuranceInvoiceAsync(int invoiceId)
    {
        var save = await _repository.DeleteInsuranceInvoiceAsync(invoiceId);
        return save;
    }
}