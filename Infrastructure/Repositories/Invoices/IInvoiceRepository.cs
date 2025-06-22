using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IInvoiceRepository
    {
        Task<decimal> GetAmountDueAsync(InvoiceDto invoice, string? UtilityType);

        Task<int> InvoiceTypeExistsAsync(string invoiceType);

    }
}
