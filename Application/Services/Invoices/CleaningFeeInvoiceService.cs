using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class CleaningFeeInvoiceService : ICleaningFeeInvoiceService
    {
        private readonly ICleaningFeeInvoiceRepository _repository;

        public CleaningFeeInvoiceService(ICleaningFeeInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateCleaningFeeInvoiceAsync(CleaningFeeInvoiceCreateDto dto)
        {
            var saved = await _repository.CreateCleaningFeeInvoiceAsync(dto);
            return saved;
        }

        public Task<CleaningFeeInvoice?> GetCleaningFeeInvoiceByIdAsync(int invoiceId)
        {
            return _repository.GetCleaningFeeInvoiceByIdAsync(invoiceId);
        }

        public Task<IEnumerable<CleaningFeeInvoice>> GetAllCleaningFeeInvoiceAsync()
        {
            return _repository.GetAllCleaningFeeInvoiceAsync();
        }

        public async Task<bool> UpdateCleaningFeeInvoiceAsync(CleaningFeeInvoiceCreateDto dto)
        {
            var existing = await _repository.GetCleaningFeeInvoiceByIdAsync(dto.InvoiceId);
            if (existing == null) return false;

            existing.Amount = dto.Amount;
            await _repository.UpdateCleaningFeeInvoiceAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int invoiceId)
        {
            var save = await _repository.DeleteCleaningFeeInvoiceAsync(invoiceId);

            return save;
        }
    }
}