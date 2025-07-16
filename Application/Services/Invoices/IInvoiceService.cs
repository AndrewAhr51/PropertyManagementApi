using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface IInvoiceService
    {
        Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId);
        Task<InvoiceDto?> GetInvoiceByTenantIdandInvoiceIdIdAsync(int tenant, int invoiceId);
        Task<List<InvoiceDto>> GetAllInvoicesAsync();
        Task<IEnumerable<OpenInvoiceByTenantDto>> GetAllInvoicesByTenantIdAsync(int tenantId);
        Task<bool> CreateInvoiceAsync(CreateInvoiceDto invoiceDto);
        Task<bool> UpdateInvoiceAsync(InvoiceDto invoiceDto);
        Task<bool> DeleteInvoiceAsync(int invoiceId);    
        Task<int> CreateLineItemAsync(CreateInvoiceLineItemDto dto);
        Task<InvoiceLineItem?> GetLineItemAsync(int lineItemId);
        Task<List<InvoiceLineItem>> GetLineItemsByInvoiceIdAsync(int invoiceId);
        Task<List<InvoiceLineItemDto>> GetLineItemsForInvoiceAsync(int invoiceId);
        Task<bool> UpdateLineItemAsync(int lineItemId, CreateInvoiceLineItemDto dto);
        Task<bool> DeleteLineItemAsync(int lineItemId);
    }
}
