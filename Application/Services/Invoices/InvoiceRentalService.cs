using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class InvoiceRentalService : IInvoiceRentalService
    {
        private readonly IInvoiceRentalRepository _repository;

        public InvoiceRentalService(IInvoiceRentalRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateInvoiceRentalAsync(InvoiceRentCreateDto dto, string invoiceType ="Rent")
        {
            var invoiceTypeId = await _repository.InvoiceTypeExistsAsync(invoiceType);
            if (invoiceTypeId == null)
            {
                throw new ArgumentException($"Invalid invoice type: {invoiceType}");
            }
            var invoice = new InvoiceRental
            {
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                DueDate = dto.DueDate,
                PropertyId = dto.PropertyId,
                InvoiceTypeId = invoiceTypeId,
                RentMonth = dto.RentMonth,
                RentYear = dto.RentYear,
                Notes = dto.Notes,
                CreatedDate = DateTime.UtcNow
            };

            return await _repository.CreateInvoiceRentalAsync(invoice);
        }

        public Task<InvoiceRental?> GetInvoiceRentalByIdAsync(int invoiceId) =>
            _repository.GetInvoiceRentalByIdAsync(invoiceId);

        public Task<IEnumerable<InvoiceRental>> GetAllInvoicesRentalsAsync() =>
            _repository.GetAllInvoiceRentalsAsync();

        public Task<IEnumerable<InvoiceRental>> GetInvoicesRentalsByMonthYearAsync(int month, int year) =>
            _repository.GetInvoiceRentalByMonthYearAsync(month, year);

        public async Task UpdateInvoiceRentalAsync(InvoiceRentCreateDto dto)
        {
            var existing = await _repository.GetInvoiceRentalByIdAsync(dto.InvoiceId);
            if (existing is null) return;

            existing.Amount = dto.Amount;
            existing.DueDate = dto.DueDate;
            existing.RentMonth = dto.RentMonth;
            existing.RentYear = dto.RentYear;
            existing.Notes = dto.Notes;

            await _repository.UpdateAsync(existing);
        }

        public Task DeleteInvoiceRentalAsync(int invoiceId) =>
            _repository.DeleteAsync(invoiceId);
    }
}