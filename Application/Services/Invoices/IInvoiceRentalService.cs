using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface IInvoiceRentalService
    {
        Task <bool> CreateInvoiceRentalAsync(InvoiceRentCreateDto dto, string invoiceType = "Rent");
        Task<InvoiceRental?> GetInvoiceRentalByIdAsync(int invoiceId);
        Task<IEnumerable<InvoiceRental>> GetAllInvoicesRentalsAsync();
        Task<IEnumerable<InvoiceRental>> GetInvoicesRentalsByMonthYearAsync(int month, int year);
        Task UpdateInvoiceRentalAsync(InvoiceRentCreateDto dto);
        Task DeleteInvoiceRentalAsync(int invoiceId);
    }
}