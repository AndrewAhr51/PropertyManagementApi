using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class ParkingFeeInvoiceService : IParkingFeeInvoiceService
    {
        private readonly IParkingFeeInvoiceRepository _repository;

        public ParkingFeeInvoiceService(IParkingFeeInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateParkingFeeInvoiceAsync(ParkingFeeInvoiceCreateDto dto)
        {
            var save = await _repository.CreateParkingFeeInvoiceAsync(dto);
            return save;
        }

        public Task<ParkingFeeInvoice?> GetParkingFeeInvoiceByIdAsync(int invoiceId)
        {
            return _repository.GetParkingFeeInvoiceByIdAsync(invoiceId);
        }

        public Task<IEnumerable<ParkingFeeInvoice>> GetAllParkingFeeInvoiceAsync()
        {
            return _repository.GetAllParkingFeeInvoiceAsync();
        }

        public async Task<bool> UpdateParkingFeeInvoiceAsync(ParkingFeeInvoiceCreateDto dto)
        {
            var existing = await _repository.GetParkingFeeInvoiceByIdAsync(dto.InvoiceId);
            if (existing == null) return false;

            existing.Amount = dto.Amount;
            existing.SpotIdentifier = dto.SpotIdentifier;

            await _repository.UpdateParkingFeeInvoiceAsync(existing);
            return true;
        }

        public async Task<bool> DeleteParkingFeeInvoiceAsync(int invoiceId)
        {
            var save = await _repository.DeleteParkingFeeInvoiceAsync(invoiceId);
            return save;
        }
    }
}