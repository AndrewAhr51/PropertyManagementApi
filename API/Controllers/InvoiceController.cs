using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Application.Services.InvoiceExport;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _repository;
        private readonly IExportService<CumulativeInvoiceDto> _exportService;

        public InvoiceController(IInvoiceRepository repository, IExportService<CumulativeInvoiceDto> exportService)
        {
            _repository = repository;
            _exportService = exportService;
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
    }
}