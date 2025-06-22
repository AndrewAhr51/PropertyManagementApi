using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface IUtilityInvoiceService
    {
        Task<bool> CreateUtilitiesInvoiceAsync(UtilityInvoiceCreateDto dto);
        Task<UtilityInvoice?> GetUtilitiesInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<UtilityInvoice>> GetUtilitiesInvoiceAllAsync();
        Task<bool> UpdateUtilitiesInvoiceAsync(UtilityInvoiceCreateDto dto);
        Task<bool> DeleteUtilitiesInvoiceAsync(int invoiceId);
    }
}