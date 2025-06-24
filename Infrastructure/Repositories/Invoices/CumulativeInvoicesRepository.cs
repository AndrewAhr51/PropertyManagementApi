using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class CumulativeInvoicesRepository : ICumulativeInvoicesRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<RentInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public CumulativeInvoicesRepository(MySqlDbContext context, ILogger<RentInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
        }
        public async Task<List<CumulativeInvoiceDto>> GetAllInvoicesForPropertyAsync(int propertyId)
        {

            List<CumulativeInvoiceDto> cumulativeInvoices = new List<CumulativeInvoiceDto>();

            List<Invoice> invoices = await _context.Invoices
                 .Where(i => i.PropertyId == propertyId)
                 .ToListAsync();

            foreach (var invoice in invoices)
            {
                string InvoiceType = await _invoiceRepository.GetInvoiceTypeNameByIdAsync(invoice.InvoiceTypeId);
                if (InvoiceType == null)
                {
                    _logger.LogWarning($"Invoice type not found for InvoiceId: {invoice.InvoiceId}");
                    continue;
                }

                cumulativeInvoices.Add(new CumulativeInvoiceDto
                {
                    InvoiceId = invoice.InvoiceId,
                    PropertyId = invoice.PropertyId,
                    CustomerName = invoice.CustomerName ?? "Unknown",
                    Amount = invoice.Amount,
                    CreatedDate = invoice.CreatedDate,
                    DueDate = invoice.DueDate,
                    Status = invoice.Status,
                    Notes = invoice.Notes,
                    InvoiceType = InvoiceType
                });
            }

            return cumulativeInvoices;
        }

    }
}
