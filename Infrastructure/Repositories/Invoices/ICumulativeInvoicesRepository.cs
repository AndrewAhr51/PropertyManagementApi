using PropertyManagementAPI.Domain.DTOs.Invoice;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public interface ICumulativeInvoicesRepository 
    {
        Task<List<CumulativeInvoiceDto>> GetAllInvoicesForPropertyAsync(int propertyId);
    }
}
