using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

public interface IPropertyTaxInvoiceRepository
{
    Task<bool> CreatePropertyTaxInvoiceAsync(PropertyTaxInvoiceCreateDto invoice);
    Task<PropertyTaxInvoice?> GetPropertyTaxInvoiceByIdAsync(int invoiceId);
    Task<IEnumerable<PropertyTaxInvoice>> GetAllPropertyTaxInvoiceAsync();
    Task UpdatePropertyTaxInvoiceAsync(PropertyTaxInvoice invoice);
    Task <bool>DeletePropertyTaxInvoiceAsync(int invoiceId);
}