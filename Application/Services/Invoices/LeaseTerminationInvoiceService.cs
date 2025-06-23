using PropertyManagementAPI.Application.Repositories.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class LeaseTerminationInvoiceService : ILeaseTerminationInvoiceService
    {
        private readonly ILeaseTerminationInvoiceRepository _repository;

        public LeaseTerminationInvoiceService(ILeaseTerminationInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateLeaseTerminationInvoiceAsync(LeaseTerminationInvoiceCreateDto dto)
        {
            var save = await _repository.CreateLeaseTerminationInvoiceAsync(dto);
            return save;
        }

        public Task<LeaseTerminationInvoice?> GetLeaseTerminationInvoiceByIdAsync(int invoiceId)
        {
            return _repository.GetLeaseTerminationInvoiceByIdAsync(invoiceId);
        }

        public Task<IEnumerable<LeaseTerminationInvoice>> GetAllLeaseTerminationInvoiceAsync()
        {
            return _repository.GetAllLeaseTerminationInvoiceAsync();
        }

        public async Task<bool> UpdateLeaseTerminationInvoiceAsync(LeaseTerminationInvoiceCreateDto dto)
        {
            var existing = await _repository.GetLeaseTerminationInvoiceByIdAsync(dto.InvoiceId);
            if (existing == null) return false;

            existing.TerminationReason = dto.TerminationReason;
            await _repository.UpdateLeaseTerminationInvoiceAsync(existing);
            return true;
        }

        public async Task<bool> DeleteLeaseTerminationInvoiceAsync(int invoiceId)
        {
           var save = await _repository.DeleteLeaseTerminationInvoiceAsync(invoiceId);
           return save;
        }
    }
}