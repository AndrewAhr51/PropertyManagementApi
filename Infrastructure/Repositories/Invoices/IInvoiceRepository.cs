using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IInvoiceRepository
    {
        Task<decimal> GetAmountDueAsync(InvoiceDto invoice, string? UtilityType);

        Task<int> InvoiceTypeExistsAsync(string invoiceType);

        Task<IEnumerable<Invoice>> GetAllInvoicesForPropertyAsync(int propertyId);

        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);

        Task<decimal> GetTotalAmountByPropertyAsync(int propertyId, string? status = null);

        Task<Dictionary<string, decimal>> GetAmountByTypeAsync(int propertyId);

        Task<Dictionary<string, decimal>> GetMonthlyTotalsAsync(int propertyId, int year);

        Task<IEnumerable<Invoice>> GetFilteredAsync(int propertyId, string? type = null, string? status = null, DateTime? dueBefore = null);

        Task<decimal> GetBalanceForwardAsync(int propertyId, DateTime asOfDate);

        Task<string> GetPropertyOwnerNameAsync(int propertyId);

        Task<int> GetInvoiceTypeIdByNameAsync(string invoiceTypeName);

        Task<string> GetInvoiceTypeNameByIdAsync(int invoiceTypeid);

        Task<List<CumulativeInvoiceDto>> ExportInvoicesByPropertyIdAsync(int propertyId);

        Task<List<CumulativeInvoiceDto>> ExportInvoicesByInvoiceIdAsync(int invoiceId);

        Task<SummaryDto> GetSummaryAsync(int propertyId);

        Task<List<CumulativeInvoiceDto>> GetByPropertyAsync(int propertyId, string type, string? status = null, DateTime? dueBefore = null);

        Task<List<CumulativeInvoiceDto>> SendCumulativeInvoiceAsync(int propertyId, string recipientEmail);

        Task<List<CumulativeInvoiceDto>> SendInvoiceAsync(int propertyId, string recipientEmail);


    }
}
