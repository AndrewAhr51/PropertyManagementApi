using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Application.Services.InvoiceExport;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using System.Collections.Generic;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {

        private readonly IInvoiceService _invoiceService;
        private readonly IExportService<CumulativeInvoiceDto> _exportService;
        private readonly IEmailService _emailService;

        public InvoiceController(IInvoiceService invoiceService, ICumulativeInvoicesRepository cummulativeInvoiceRepository, IExportService<CumulativeInvoiceDto> exportService, IEmailService emailService)
        {
            _invoiceService = invoiceService;
            _exportService = exportService;
            _emailService = emailService;

        }

        [HttpGet("property/{propertyId:int}")]
        public async Task<IActionResult> GetByProperty(int propertyId, [FromQuery] string? type = null, [FromQuery] string? status = null, [FromQuery] DateTime? dueBefore = null)
        {
            var invoices = await _invoiceService.GetFilteredAsync(propertyId, type, status, dueBefore);
            return Ok(invoices);
        }

        [HttpGet("property/{propertyId:int}/summary")]
        public async Task<SummaryDto> GetSummaryAsync(int propertyId)
        {
            SummaryDto summaryDto = (SummaryDto)await _invoiceService.GetSummaryAsync(propertyId);
            return summaryDto;
        }

        [HttpGet("property/{propertyId:int}/balance-forward")]
        public async Task<ActionResult<decimal>> GetBalanceForward(int propertyId, [FromQuery] DateTime asOfDate)
        {
            var balance = await _invoiceService.GetBalanceForwardAsync(propertyId, asOfDate);
            return Ok(balance);
        }

        [HttpGet("property/{propertyId:int}/export/pdf")]
        public async Task<IActionResult> ExportPdf(int propertyId)
        {
            var dto = await _invoiceService.ExportInvoicesByPropertyIdAsync(propertyId);
            if (dto == null || !dto.Any())
            {
                return NotFound("No invoices found for the specified property.");
            }

            var pdfBytes = await _exportService.ExportToPdfAsync(dto);
            return File(pdfBytes, "application/pdf", $"invoices_{propertyId}.pdf");
        }

        [HttpGet("property/{propertyId:int}/export/excel")]
        public async Task<IActionResult> ExportExcel(int propertyId)
        {
            var dto = await _invoiceService.ExportInvoicesByPropertyIdAsync(propertyId);
            if (dto == null || !dto.Any())
            {
                return NotFound("No invoices found for the specified property.");
            }

            var excelBytes = await _exportService.ExportToExcelAsync(dto);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"invoices_{propertyId}.xlsx");
        }

        [HttpGet("property/{propertyId:int}/export/quickbooks")]
        public async Task<IActionResult> ExportQuickBooksCsv(int propertyId)
        {
            var dto = await _invoiceService.ExportInvoicesByPropertyIdAsync(propertyId);
            if (dto == null || !dto.Any())
            {
                return NotFound("No invoices found for the specified property.");
            }

            var csvBytes = await _exportService.ExportToCsvAsync(dto); // Call the correct method
            return File(csvBytes, "text/csv", $"quickbooks_invoices_{propertyId}.csv");
        }

        [HttpPost("send-invoice/{invoiceId:int}")]
        public async Task<IActionResult> SendInvoice(int invoiceId, [FromQuery] string recipientEmail)
        {
            var invoices = await _invoiceService.SendInvoiceAsync(invoiceId, recipientEmail);
            return Ok("Invoice email sent successfully.");
        }

        [HttpPost("send-cumultive-invoice/{invoiceId:int}")]
        public async Task<IActionResult> SendCummulativeInvoice(int propertyId, [FromQuery] string recipientEmail)
        {
            var invoices = await _invoiceService.SendCumulativeInvoiceAsync(propertyId, recipientEmail);

            return Ok("Invoice email sent successfully.");
        }
    }
}