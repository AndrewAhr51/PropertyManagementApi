using PropertyManagementAPI.Domain.DTOs;

public interface IInvoiceService
{
    Task<InvoiceDto> GetInvoiceByIdAsync(int invoiceId);
    Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
    Task<IEnumerable<InvoiceDto>> GetInvoicesByTenantIdAsync(int tenantId);
    Task<bool> CreateInvoiceAsync(InvoiceDto invoiceDto);
    Task<bool> UpdateInvoiceAsync(InvoiceDto invoiceDto);
    Task<bool> DeleteInvoiceAsync(int invoiceId);
}