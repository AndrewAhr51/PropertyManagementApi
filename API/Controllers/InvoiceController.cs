using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Application.Services.InvoiceExport;
using PropertyManagementAPI.Application.Services;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _repository;
        private readonly IExportService<CumulativeInvoiceDto> _exportService;
        private readonly IEmailService _emailService; // Add an instance of IEmailService

        public InvoiceController(IInvoiceRepository repository, IExportService<CumulativeInvoiceDto> exportService, IEmailService emailService)
        {
            _repository = repository;
            _exportService = exportService;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        }

        [HttpGet("property/{propertyId:int}")]
        public async Task<IActionResult> GetByProperty(int propertyId, [FromQuery] string? type = null, [FromQuery] string? status = null, [FromQuery] DateTime? dueBefore = null)
        {
            var invoices = await _repository.GetFilteredAsync(propertyId, type, status, dueBefore);

            var result = invoices.Select(i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                DueDate = i.DueDate,
                Status = i.Status,
                Notes = i.Notes,
                InvoiceType = i.GetType().Name
            });

            return Ok(result);
        }

        [HttpGet("property/{propertyId:int}/summary")]
        public async Task<IActionResult> GetSummary(int propertyId)
        {
            var invoices = await _repository.GetAllInvoicesForPropertyAsync(propertyId);
            var dto = invoices.Select(i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                DueDate = i.DueDate,
                Status = i.Status,
                Notes = i.Notes,
                InvoiceType = i.GetType().Name
            });

            var grouped = dto.GroupBy(d => d.InvoiceType)
                             .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            var monthly = dto.GroupBy(d => d.CreatedDate.ToString("yyyy-MM"))
                             .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            return Ok(new
            {
                TotalAmount = dto.Sum(x => x.Amount),
                Count = dto.Count(),
                BreakdownByType = grouped,
                MonthlyTotals = monthly
            });
        }

        [HttpGet("property/{propertyId:int}/balance-forward")]
        public async Task<ActionResult<decimal>> GetBalanceForward(int propertyId, [FromQuery] DateTime asOfDate)
        {
            var balance = await _repository.GetBalanceForwardAsync(propertyId, asOfDate);
            return Ok(balance);
        }

        [HttpGet("property/{propertyId:int}/export/pdf")]
        public async Task<IActionResult> ExportPdf(int propertyId)
        {
            var invoices = await _repository.GetAllInvoicesForPropertyAsync(propertyId);
            var dto = invoices.Select(i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                Notes = i.Notes,
                InvoiceType = i.GetType().Name
            });

            var pdfBytes = await _exportService.ExportToPdfAsync(dto);
            return File(pdfBytes, "application/pdf", $"invoices_{propertyId}.pdf");
        }

        [HttpGet("property/{propertyId:int}/export/excel")]
        public async Task<IActionResult> ExportExcel(int propertyId)
        {
            var invoices = await _repository.GetAllInvoicesForPropertyAsync(propertyId);
            var dto = invoices.Select(i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                Notes = i.Notes,
                InvoiceType = i.GetType().Name
            });

            var excelBytes = await _exportService.ExportToExcelAsync(dto);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"invoices_{propertyId}.xlsx");
        }

        [HttpGet("property/{propertyId:int}/export/quickbooks")]
        public async Task<IActionResult> ExportQuickBooksCsv(int propertyId)
        {
            var invoices = await _repository.GetAllInvoicesForPropertyAsync(propertyId);
            var dto = invoices.Select(i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                CustomerName = i.CustomerName,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                Notes = i.Notes,
                InvoiceType = i.GetType().Name
            });

            var csvBytes = await _exportService.ExportToCsvAsync(dto); // Call the correct method
            return File(csvBytes, "text/csv", $"quickbooks_invoices_{propertyId}.csv");
        }

        [HttpPost("send-invoice/{invoiceId:int}")]
        public async Task<IActionResult> SendInvoice(int invoiceId, [FromQuery] string recipientEmail)
        {
            var invoice = await _repository.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null) return NotFound("Invoice not found.");

            var invoiceType = await _repository.GetInvoiceTypeNameByIdAsync(invoice.InvoiceTypeId);
            if (invoice == null) return NotFound("Invoice not found.");

            // Map Invoice to CumulativeInvoiceDto
            var cumulativeInvoiceDto = new CumulativeInvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                PropertyId = invoice.PropertyId,
                CustomerName = invoice.CustomerName,
                Amount = invoice.Amount,
                CreatedDate = invoice.CreatedDate,
                DueDate = invoice.DueDate,
                Notes = invoice.Notes,
                Status = invoice.Status,
                InvoiceType = invoiceType
            };

            var pdfBytes = await _exportService.ExportToPdfAsync(new List<CumulativeInvoiceDto> { cumulativeInvoiceDto });
            var pdfPath = Path.Combine(Path.GetTempPath(), $"invoice_{invoiceId}.pdf");
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes); // Use System.IO.File explicitly to avoid ambiguity

            await _emailService.SendInvoiceEmailAsync(recipientEmail, pdfPath); // Use the instance of IEmailService

            return Ok("Invoice email sent successfully.");
        }
    }
}