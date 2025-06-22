using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IInvoiceRentalRepository
    {
        Task<bool>CreateInvoiceRentalAsync(InvoiceRental invoice);
        Task<InvoiceRental?> GetInvoiceRentalByIdAsync(int invoiceId);
        Task<IEnumerable<InvoiceRental>> GetAllInvoiceRentalsAsync();
        Task<IEnumerable<InvoiceRental>> GetInvoiceRentalByMonthYearAsync(int month, int year);
        Task UpdateAsync(InvoiceRental invoice);
        Task DeleteAsync(int invoiceId);
        Task<int> InvoiceTypeExistsAsync(string invoiceType);

    }
}