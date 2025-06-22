using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface IInvoiceRentalService
    {
        Task <bool> CreateInvoiceRentalAsync(RentInvoiceCreateDto dto, string invoiceType = "Rent");
        Task<RentInvoice?> GetInvoiceRentalByIdAsync(int invoiceId);
        Task<IEnumerable<RentInvoice>> GetAllInvoicesRentalsAsync();
        Task<IEnumerable<RentInvoice>> GetInvoicesRentalsByMonthYearAsync(int month, int year);
        Task UpdateInvoiceRentalAsync(RentInvoiceCreateDto dto);
        Task DeleteInvoiceRentalAsync(int invoiceId);
    }
}