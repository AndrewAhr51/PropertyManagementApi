using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
        Task<List<Invoice>> GetAllInvoicesAsync();
        Task<bool> CreateInvoiceAsync(CreateInvoiceDto invoice);
        Task<bool>UpdateInvoiceAsync(InvoiceDto invoice);
        Task<bool>DeleteInvoiceAsync(int invoiceId);
        Task<int> CreateLineItemAsync(CreateInvoiceLineItemDto dto);
        Task<List<InvoiceLineItemDto>> GetLineItemsForInvoiceAsync(int invoiceId);
        Task<List<InvoiceLineItemMetadata>> GetMetadataByLineItemIdAsync(int lineItemId);
        Task<InvoiceLineItem?> GetLineItemAsync(int lineItemId);
        Task<List<InvoiceLineItem>> GetLineItemsByInvoiceIdAsync(int invoiceId);
        Task<bool> UpdateLineItemAsync(int lineItemId, CreateInvoiceLineItemDto dto);
        Task<bool> DeleteLineItemAsync(int lineItemId);
        Task<InvoiceDto?> MapInvoiceToDto(Invoice invoice);

    }
}
