using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface ICleaningFeeInvoiceService
    {
        Task<bool> CreateCleaningFeeInvoiceAsync(CleaningFeeInvoiceCreateDto dto);
        Task<CleaningFeeInvoice?> GetCleaningFeeInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<CleaningFeeInvoice>> GetAllCleaningFeeInvoiceAsync();
        Task<bool> UpdateCleaningFeeInvoiceAsync(CleaningFeeInvoiceCreateDto dto);
        Task<bool> DeleteAsync(int invoiceId);
    }
}