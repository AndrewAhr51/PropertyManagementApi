using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IUtilityInvoiceRepository
    {
        Task <bool>CreateUtilitiesInvoiceAsync(UtilityInvoiceCreateDto invoice);
        Task<UtilityInvoice?> GetUtilitiesInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<UtilityInvoice>> GetAllUtilitiesInvoiceAsync();
        Task UpdateUtilitiesInvoiceAsync(UtilityInvoice invoice);
        Task <bool>DeleteUtilitiesInvoiceAsync(int invoiceId);
    }
}