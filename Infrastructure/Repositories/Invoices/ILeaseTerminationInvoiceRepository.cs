using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Repositories.Invoices
{
    public interface ILeaseTerminationInvoiceRepository
    {
        Task<bool> CreateLeaseTerminationInvoiceAsync(LeaseTerminationInvoiceCreateDto invoice);
        Task<LeaseTerminationInvoice?> GetLeaseTerminationInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<LeaseTerminationInvoice>> GetAllLeaseTerminationInvoiceAsync();
        Task UpdateLeaseTerminationInvoiceAsync(LeaseTerminationInvoice invoice);
        Task<bool> DeleteLeaseTerminationInvoiceAsync(int invoiceId);
    }
}