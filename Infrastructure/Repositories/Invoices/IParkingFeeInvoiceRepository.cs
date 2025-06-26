using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface IParkingFeeInvoiceRepository
    {
        Task<bool> CreateParkingFeeInvoiceAsync(ParkingFeeInvoiceCreateDto invoice);
        Task<ParkingFeeInvoice?> GetParkingFeeInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<ParkingFeeInvoice>> GetAllParkingFeeInvoiceAsync();
        Task UpdateParkingFeeInvoiceAsync(ParkingFeeInvoice invoice);
        Task <bool>DeleteParkingFeeInvoiceAsync(int invoiceId);
    }
}