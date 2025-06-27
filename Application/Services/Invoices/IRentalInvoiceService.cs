using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface IRentalInvoiceService
    {
        Task <bool> CreateInvoiceRentalAsync(RentInvoiceCreateDto dto);
        Task<RentInvoice?> GetInvoiceRentalByIdAsync(int invoiceId);
        Task<IEnumerable<RentInvoice>> GetAllInvoicesRentalsAsync();
        Task<IEnumerable<RentInvoice>> GetInvoicesRentalsByMonthYearAsync(int month, int year);
        Task <bool>UpdateInvoiceRentalAsync(RentInvoiceCreateDto dto);
        Task <bool>DeleteInvoiceRentalAsync(int invoiceId);
    }
}