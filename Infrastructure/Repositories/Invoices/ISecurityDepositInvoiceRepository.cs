using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface ISecurityDepositInvoiceRepository
    {
        Task<bool> CreateSecurityDepositInvoiceAsync(SecurityDepositInvoiceCreateDto invoice);
        Task<SecurityDepositInvoice?> GetSecurityDepositInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<SecurityDepositInvoice>> GetAllSecurityDepositInvoiceAsync();
        Task UpdateSecurityDepositInvoiceAsync(SecurityDepositInvoice invoice);
        Task<bool>DeleteSecurityDepositInvoiceAsync(int invoiceId);
    }
}