using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Email;
using PropertyManagementAPI.Application.Services.InvoiceExport;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class InvoiceService: IInvoiceService // Extend ControllerBase to use File method
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IExportService<CumulativeInvoiceDto> _exportService;
       
        public InvoiceService(IInvoiceRepository invoiceRepository, IExportService<CumulativeInvoiceDto> exportService, IEmailService emailService)
        {
            _invoiceRepository = invoiceRepository;
            _exportService = exportService;
        }

        public async Task<List<CumulativeInvoiceDto>> ExportInvoicesByPropertyIdAsync(int propertyId)
        {
           return await _invoiceRepository.ExportInvoicesByPropertyIdAsync(propertyId);
        }

        public async Task<List<CumulativeInvoiceDto>> ExportInvoicesByInvoiceIdAsync(int invoiceId)
        {
            return await _invoiceRepository.ExportInvoicesByInvoiceIdAsync(invoiceId);
        }

        public async Task<decimal> GetBalanceForwardAsync(int propertyId, DateTime asOfDate)
        {
            return await _invoiceRepository.GetBalanceForwardAsync(propertyId, asOfDate);
        }

        public async Task<List<CumulativeInvoiceDto>> GetByPropertyAsync(int propertyId, string type, string? status = null, DateTime? dueBefore = null)
        {
            return await _invoiceRepository.GetByPropertyAsync(propertyId, type, status, dueBefore);
        }

        public Task<SummaryDto> GetSummaryAsync(int propertyId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CumulativeInvoiceDto>> SendCumulativeInvoiceAsync(int propertyId, string recipientEmail)
        {
            return await _invoiceRepository.SendCumulativeInvoiceAsync(propertyId, recipientEmail);
        }

        public async Task<List<CumulativeInvoiceDto>> SendInvoiceAsync(int invoiceId, [FromQuery] string recipientEmail)
        {
            return await _invoiceRepository.SendInvoiceAsync(invoiceId, recipientEmail);
        }

        public Task<List<CumulativeInvoiceDto>> GetAllInvoicesForPropertyAsync(int propertyId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Invoice>> GetFilteredAsync(int propertyId, string? type, string? status, DateTime? dueBefore)
        {
            List<Invoice> invoice = (List<Invoice>)await _invoiceRepository.GetFilteredAsync(propertyId, type, status, dueBefore);   

            return invoice;
        }
    }
}
