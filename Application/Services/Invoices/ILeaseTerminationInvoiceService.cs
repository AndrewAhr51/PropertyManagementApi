using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface ILeaseTerminationInvoiceService
    {
        Task<bool> CreateLeaseTerminationInvoiceAsync(LeaseTerminationInvoiceCreateDto dto);
        Task<LeaseTerminationInvoice?> GetLeaseTerminationInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<LeaseTerminationInvoice>> GetAllLeaseTerminationInvoiceAsync();
        Task<bool> UpdateLeaseTerminationInvoiceAsync(LeaseTerminationInvoiceCreateDto dto);
        Task<bool> DeleteLeaseTerminationInvoiceAsync(int invoiceId);
    }
}