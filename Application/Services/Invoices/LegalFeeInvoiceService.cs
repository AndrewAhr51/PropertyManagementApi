using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices;
public class LegalFeeInvoiceService : ILegalFeeInvoiceService
{
    private readonly ILegalFeeInvoiceRepository _repository;

    public LegalFeeInvoiceService(ILegalFeeInvoiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CreateLegalFeeInvoiceAsync(LegalFeeInvoiceCreateDto dto)
    {
        var save =  await _repository.CreateLegalFeeInvoiceAsync(dto);
        return save;
    }

    public Task<LegalFeeInvoice?> GetByLegalFeeInvoiceIdAsync(int invoiceId) =>
        _repository.GetLegalFeeInvoiceByIdAsync(invoiceId);

    public Task<IEnumerable<LegalFeeInvoice>> GetAllLegalFeeInvoiceAsync() =>
        _repository.GetAllLegalFeeInvoiceAsync();

    public async Task<bool> UpdateLegalFeeInvoiceAsync(LegalFeeInvoiceCreateDto dto)
    {
        await _repository.UpdateLegalFeeInvoiceAsync(dto);
        return true;
    }

    public async Task<bool> DeleteLegalFeeInvoiceAsync(int invoiceId)
    {
        var save =  await _repository.DeleteLegalFeeInvoiceAsync(invoiceId);
        return save;
    }

}