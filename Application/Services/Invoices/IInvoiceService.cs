using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<List<CumulativeInvoiceDto>> GetByPropertyAsync(int propertyId, string type, string? status = null, DateTime? dueBefore = null);
        Task<SummaryDto> GetSummaryAsync(int propertyId);
        Task<decimal> GetBalanceForwardAsync(int propertyId, DateTime asOfDate);
        Task<List<CumulativeInvoiceDto>> ExportInvoicesByPropertyIdAsync(int propertyId);
        Task<List<CumulativeInvoiceDto>> ExportInvoicesByInvoiceIdAsync(int propertyId);
        Task<List<CumulativeInvoiceDto>> SendInvoiceAsync(int invoiceId, [FromQuery] string recipientEmail);
        Task<List<CumulativeInvoiceDto>> SendCumulativeInvoiceAsync(int propertyId, [FromQuery] string recipientEmail);
        Task<List<CumulativeInvoiceDto>> GetAllInvoicesForPropertyAsync(int propertyId);
        Task<IEnumerable<Invoice>> GetFilteredAsync(int propertyId, [FromQuery] string? type = null, [FromQuery] string? status = null, [FromQuery] DateTime? dueBefore = null);
        
    }
}
