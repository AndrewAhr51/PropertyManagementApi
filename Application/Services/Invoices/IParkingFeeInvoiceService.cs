using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface IParkingFeeInvoiceService
    {
        Task<bool> CreateParkingFeeInvoiceAsync(ParkingFeeInvoiceCreateDto dto);
        Task<ParkingFeeInvoice?> GetParkingFeeInvoiceByIdAsync(int invoiceId);
        Task<IEnumerable<ParkingFeeInvoice>> GetAllParkingFeeInvoiceAsync();
        Task<bool> UpdateParkingFeeInvoiceAsync(ParkingFeeInvoiceCreateDto dto);
        Task<bool> DeleteParkingFeeInvoiceAsync(int invoiceId);
    }
}