using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface ICleaningFeeInvoiceRepository
    {
        Task<bool> CreateCleaningFeeInvoiceAsync(CleaningFeeInvoiceCreateDto invoice);
        Task<CleaningFeeInvoice?> GetCleaningFeeInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<CleaningFeeInvoice>> GetAllCleaningFeeInvoiceAsync();
        Task UpdateCleaningFeeInvoiceAsync(CleaningFeeInvoice invoice);
        Task <bool>DeleteCleaningFeeInvoiceAsync(int invoiceId);
    }
}