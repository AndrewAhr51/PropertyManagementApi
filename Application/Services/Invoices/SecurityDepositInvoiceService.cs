using PropertyManagementAPI.Application.Repositories.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class SecurityDepositInvoiceService : ISecurityDepositInvoiceService
    {
        private readonly ISecurityDepositInvoiceRepository _repository;

        public SecurityDepositInvoiceService(ISecurityDepositInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateSecurityDepositInvoiceAsync(SecurityDepositInvoiceCreateDto dto)
        {
            await _repository.CreateSecurityDepositInvoiceAsync(dto);
            return true;
        }

        public Task<SecurityDepositInvoice?> GetSecurityDepositInvoiceByIdAsync(int invoiceId)
        {
            return _repository.GetSecurityDepositInvoiceByIdAsync(invoiceId);
        }

        public Task<IEnumerable<SecurityDepositInvoice>> GetAllSecurityDepositInvoiceAsync()
        {
            return _repository.GetAllSecurityDepositInvoiceAsync();
        }

        public async Task<bool> UpdateSecurityDepositInvoiceAsync(SecurityDepositInvoiceCreateDto dto)
        {
            var existing = await _repository.GetSecurityDepositInvoiceByIdAsync(dto.InvoiceId);
            if (existing == null) return false;

            existing.IsRefundable = dto.IsRefundable;
            await _repository.UpdateSecurityDepositInvoiceAsync(existing);
            return true;
        }

        public async Task<bool> DeleteSecurityDepositInvoiceAsync(int invoiceId)
        {
            var existing = await _repository.GetSecurityDepositInvoiceByIdAsync(invoiceId);
            if (existing == null) return false;

            await _repository.DeleteSecurityDepositInvoiceAsync(invoiceId);
            return true;
        }
    }
}