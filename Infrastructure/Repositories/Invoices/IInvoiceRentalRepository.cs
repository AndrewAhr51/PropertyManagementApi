using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IInvoiceRentalRepository
    {
        Task<bool>CreateInvoiceRentalAsync(RentInvoice invoice);
        Task<RentInvoice?> GetInvoiceRentalByIdAsync(int invoiceId);
        Task<IEnumerable<RentInvoice>> GetAllInvoiceRentalsAsync();
        Task<IEnumerable<RentInvoice>> GetInvoiceRentalByMonthYearAsync(int month, int year);
        Task UpdateAsync(RentInvoice invoice);
        Task DeleteAsync(int invoiceId);
        Task<int> InvoiceTypeExistsAsync(string invoiceType);

    }
}