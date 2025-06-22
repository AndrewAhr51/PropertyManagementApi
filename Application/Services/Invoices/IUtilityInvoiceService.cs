using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface IUtilityInvoiceService
    {
        Task<bool> CreateAsync(UtilityInvoiceCreateDto dto, string invoiceType = "Utilities");
        Task<UtilityInvoice?> GetByIdAsync(int invoiceId);
        Task<IEnumerable<UtilityInvoice>> GetAllAsync();
        Task<bool> UpdateAsync(UtilityInvoiceCreateDto dto);
        Task<bool> DeleteAsync(int invoiceId);
    }
}