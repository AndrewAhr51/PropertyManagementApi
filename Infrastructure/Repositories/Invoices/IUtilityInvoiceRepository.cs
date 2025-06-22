using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Repositories.Invoices
{
    public interface IUtilityInvoiceRepository
    {
        Task CreateAsync(UtilityInvoice invoice);
        Task<UtilityInvoice?> GetByIdAsync(int invoiceId);
        Task<IEnumerable<UtilityInvoice>> GetAllAsync();
        Task UpdateAsync(UtilityInvoice invoice);
        Task DeleteAsync(int invoiceId);
    }
}