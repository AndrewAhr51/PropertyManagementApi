using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface ISecurityDepositInvoiceService
    {
        Task<bool> CreateSecurityDepositInvoiceAsync(SecurityDepositInvoiceCreateDto dto);
        Task<SecurityDepositInvoice?> GetSecurityDepositInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<SecurityDepositInvoice>> GetAllSecurityDepositInvoiceAsync();
        Task<bool> UpdateSecurityDepositInvoiceAsync(SecurityDepositInvoiceCreateDto dto);
        Task<bool> DeleteSecurityDepositInvoiceAsync(int invoiceId);
    }
}