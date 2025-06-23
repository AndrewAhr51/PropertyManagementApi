using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

public interface IPropertyTaxInvoiceService
{
    Task<bool> CreatePropertyTaxInvoiceAsync(PropertyTaxInvoiceCreateDto dto);
    Task<PropertyTaxInvoice?> GetByPropertyTaxInvoiceIdAsync(int invoiceId);
    Task<IEnumerable<PropertyTaxInvoice>> GetAllPropertyTaxInvoiceAsync();
    Task<bool> UpdatePropertyTaxInvoiceAsync(PropertyTaxInvoiceCreateDto dto);
    Task<bool> DeletePropertyTaxInvoiceAsync(int invoiceId);
}