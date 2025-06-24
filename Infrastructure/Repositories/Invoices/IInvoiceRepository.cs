using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IInvoiceRepository
    {
        Task<decimal> GetAmountDueAsync(InvoiceDto invoice, string? UtilityType);

        Task<int> InvoiceTypeExistsAsync(string invoiceType);

        Task<IEnumerable<Invoice>> GetAllInvoicesForPropertyAsync(int propertyId);

        Task<IEnumerable<Invoice>> GetAllInvoicesForPropertyAsync(int propertyId, string? status = null);

        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);

        Task<decimal> GetTotalAmountByPropertyAsync(int propertyId, string? status = null);

        Task<Dictionary<string, decimal>> GetAmountByTypeAsync(int propertyId);

        Task<Dictionary<string, decimal>> GetMonthlyTotalsAsync(int propertyId, int year);

        Task<IEnumerable<Invoice>> GetFilteredAsync(int propertyId, string? type = null, string? status = null, DateTime? dueBefore = null);

        Task<decimal> GetBalanceForwardAsync(int propertyId, DateTime asOfDate);

    }
}
