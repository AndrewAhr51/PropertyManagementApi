using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;


namespace PropertyManagementAPI.Application.Services.Invoices;
public interface ILegalFeeInvoiceService
{
    Task<bool> CreateLegalFeeInvoiceAsync(LegalFeeInvoiceCreateDto dto);
    Task<LegalFeeInvoice?> GetByLegalFeeInvoiceIdAsync(int invoiceId);
    Task<IEnumerable<LegalFeeInvoice>> GetAllLegalFeeInvoiceAsync();
    Task<bool> UpdateLegalFeeInvoiceAsync(LegalFeeInvoiceCreateDto dto);
    Task<bool> DeleteLegalFeeInvoiceAsync(int invoiceId);
}