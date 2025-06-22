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

        public async Task<bool> CreateAsync(UtilityInvoiceCreateDto dto, string invoiceType = "Utilities")
        {
            var invoice = new UtilityInvoice
            {
                InvoiceId = dto.InvoiceId,
                UtilityType = dto.UtilityType,
                UsageAmount = dto.UsageAmount
            };

            await _repository.CreateAsync(invoice);
            return true;
        }

        public Task<UtilityInvoice?> GetByIdAsync(int invoiceId)
        {
            return _repository.GetByIdAsync(invoiceId);
        }

        public Task<IEnumerable<UtilityInvoice>> GetAllAsync()
        {
            return _repository.GetAllAsync();
        }

        public async Task<bool> UpdateAsync(UtilityInvoiceCreateDto dto)
        {
            var existing = await _repository.GetByIdAsync(dto.InvoiceId);
            if (existing == null) return false;

            existing.UtilityType = dto.UtilityType;
            existing.UsageAmount = dto.UsageAmount;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public Task<bool> DeleteAsync(int invoiceId)
        {
            return _repository.DeleteAsync(invoiceId)
                .ContinueWith(t => t.IsCompletedSuccessfully);
        }
    }
}