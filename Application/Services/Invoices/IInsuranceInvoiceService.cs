using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

public interface IInsuranceInvoiceService
{
    Task<bool> CreateInsuranceInvoiceAsync(InsuranceInvoiceCreateDto dto);
    Task<InsuranceInvoice?> GetInsuranceInvoiceByIdAsync(int invoiceId);
    Task<IEnumerable<InsuranceInvoice>> GetAllInsuranceInvoiceAsync();
    Task<bool> UpdateInsuranceInvoiceAsync(InsuranceInvoiceCreateDto dto);
    Task<bool> DeleteInsuranceInvoiceAsync(int invoiceId);
}