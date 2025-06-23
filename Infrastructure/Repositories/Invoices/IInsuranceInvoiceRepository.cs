using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

public interface IInsuranceInvoiceRepository
{
    Task<bool> CreateInsuranceInvoiceAsync(InsuranceInvoiceCreateDto invoice);
    Task<InsuranceInvoice?> GetInsuranceInvoiceByIdAsync(int invoiceId);
    Task<IEnumerable<InsuranceInvoice>> GetAllInsuranceInvoiceAsync();
    Task UpdateInsuranceInvoiceAsync(InsuranceInvoice invoice);
    Task<bool> DeleteInsuranceInvoiceAsync(int invoiceId);
}