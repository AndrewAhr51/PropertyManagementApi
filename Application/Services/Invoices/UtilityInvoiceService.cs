using PropertyManagementAPI.Application.Repositories.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class UtilityInvoiceService : IUtilityInvoiceService
    {
        private readonly IUtilityInvoiceRepository _repository;

        public UtilityInvoiceService(IUtilityInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateUtilitiesInvoiceAsync(UtilityInvoiceCreateDto dto)
        {
            await _repository.CreateUtilitiesInvoiceAsync(dto);
            return true;
        }

        public Task<UtilityInvoice?> GetUtilitiesInvoiceByIdAsync(int invoiceId)
        {
            return _repository.GetUtilitiesInvoiceByIdAsync(invoiceId);
        }

        public Task<IEnumerable<UtilityInvoice>> GetUtilitiesInvoiceAllAsync()
        {
            return _repository.GetAllUtilitiesInvoiceAsync();
        }

        public async Task<bool> UpdateUtilitiesInvoiceAsync(UtilityInvoiceCreateDto dto)
        {
            var existing = await _repository.GetUtilitiesInvoiceByIdAsync(dto.InvoiceId);
            if (existing == null) return false;

            existing.UsageAmount = dto.UsageAmount;

            await _repository.UpdateUtilitiesInvoiceAsync(existing);
            return true;
        }

        public async Task<bool> DeleteUtilitiesInvoiceAsync(int invoiceId)
        {
            var save = await _repository.DeleteUtilitiesInvoiceAsync(invoiceId);
            return save;            
        }
    }
}