using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class RentalInvoiceService : IRentalInvoiceService
    {
        private readonly IRentInvoiceRepository _repository;

        public RentalInvoiceService(IRentInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateInvoiceRentalAsync(RentInvoiceCreateDto dto)
        {
            return await _repository.CreateInvoiceRentalAsync(dto);
        }

        public Task<RentInvoice?> GetInvoiceRentalByIdAsync(int invoiceId) =>
            _repository.GetInvoiceRentalByIdAsync(invoiceId);

        public Task<IEnumerable<RentInvoice>> GetAllInvoicesRentalsAsync() =>
            _repository.GetAllInvoiceRentalsAsync();

        public Task<IEnumerable<RentInvoice>> GetInvoicesRentalsByMonthYearAsync(int month, int year) =>
            _repository.GetInvoiceRentalByMonthYearAsync(month, year);

        public async Task<bool> UpdateInvoiceRentalAsync(RentInvoiceCreateDto dto)
        {
            var save = await _repository.UpdateInvoiceRentalAsync(dto);
            return save;
        }

        public async Task<bool> DeleteInvoiceRentalAsync(int invoiceId)
        {
            var save = await _repository.DeleteInvoiceRentalAsync(invoiceId);
            return save;
        }
    }
}