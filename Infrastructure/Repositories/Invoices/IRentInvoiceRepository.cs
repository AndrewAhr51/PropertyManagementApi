using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IRentInvoiceRepository
    {
        Task<bool>CreateInvoiceRentalAsync(RentInvoiceCreateDto invoice);
        Task<RentInvoice?> GetInvoiceRentalByIdAsync(int invoiceId);
        Task<IEnumerable<RentInvoice>> GetAllInvoiceRentalsAsync();
        Task<IEnumerable<RentInvoice>> GetInvoiceRentalByMonthYearAsync(int month, int year);
        Task<bool> UpdateInvoiceRentalAsync(RentInvoiceCreateDto rentInvoice);
        Task<bool> DeleteInvoiceRentalAsync(int invoiceId);
    }
}