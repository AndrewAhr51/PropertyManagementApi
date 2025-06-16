using PropertyManagementAPI.Domain.Entities;

public interface IInvoiceRepository
{
    Task<Invoice> GetInvoiceByIdAsync(int invoiceId);
    Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
    Task<IEnumerable<Invoice>> GetInvoicesByTenantIdAsync(int tenantId);
    Task<bool> AddInvoiceAsync(Invoice invoice);
    Task<bool> UpdateInvoiceAsync(Invoice invoice);
    Task<bool> DeleteInvoiceAsync(int invoiceId); // Restored hard deletion
}