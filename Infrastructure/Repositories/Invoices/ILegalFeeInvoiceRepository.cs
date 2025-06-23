using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface ILegalFeeInvoiceRepository
    {
        Task<bool> CreateLegalFeeInvoiceAsync(LegalFeeInvoiceCreateDto invoice);
        Task<LegalFeeInvoice?> GetLegalFeeInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<LegalFeeInvoice>> GetAllLegalFeeInvoiceAsync();
        Task<bool> UpdateLegalFeeInvoiceAsync(LegalFeeInvoiceCreateDto dto);
        Task<bool> DeleteLegalFeeInvoiceAsync(int invoiceId);
    }
}
