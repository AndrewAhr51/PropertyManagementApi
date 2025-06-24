using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class CummulativeInvoicesService : ICummulativeInvoicesService
    {
        private readonly ICumulativeInvoicesRepository _repository;
        public CummulativeInvoicesService(ICumulativeInvoicesRepository repository)
        {
            _repository = repository;

        }
       
        public Task<List<CumulativeInvoiceDto>> GetAllInvoicesForPropertyAsync(int propertyId)
        {
            return _repository.GetAllInvoicesForPropertyAsync(propertyId);
        }

    }
}
