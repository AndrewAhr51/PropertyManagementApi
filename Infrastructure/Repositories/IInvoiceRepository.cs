using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Domain.Entities.Invoices;

public interface IInvoiceRepository<T> where T : Invoice
{
    Task<Invoice> GetInvoiceByIdAsync(int invoiceId);
    Task<Invoice> GetMostRecentInvoiceByPropertyIdAsync(int propertyId);
    Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
    Task<IEnumerable<Invoice>> GetInvoicesByPropertyIdAsync(int propertyId);
    Task<bool> CreateRentalInvoiceAsync(InvoiceRentalCreateDto rentInvoice);
    Task<bool> UpdateInvoiceAsync(Invoice invoice);
    Task<bool> DeleteInvoiceAsync(int invoiceId); // Restored hard deletion
    Task<Lease> GetLeaseInformationAsync(int propertyId); // Restored hard deletion
}