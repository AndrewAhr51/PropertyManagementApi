using PropertyManagementAPI.Domain.DTOs.Invoice;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public interface ICummulativeInvoicesService
    {
        Task <List<CumulativeInvoiceDto>> GetAllInvoicesForPropertyAsync(int propertyId);
        
    }
}
