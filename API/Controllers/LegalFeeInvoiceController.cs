using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.InvoiceOLD;
using PropertyManagementAPI.Domain.DTOs.InvoiceOLD;
using PropertyManagementAPI.Domain.Entities.InvoiceOLD;

namespace PropertyManagementAPI.API.Controllers.Invoices
{
    [ApiController]
    [Route("api/[controller]")]
    public class LegalFeeInvoiceController : ControllerBase
    {
        private readonly ILegalFeeInvoiceService _service;
        private readonly ILogger<LegalFeeInvoiceController> _logger;

        public LegalFeeInvoiceController(
            ILegalFeeInvoiceService service,
            ILogger<LegalFeeInvoiceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LegalFeeInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateLegalFeeInvoiceAsync(dto);
            if (!result)
                return StatusCode(500, "Failed to create Legal Fee Invoice.");

            return CreatedAtAction(nameof(GetById), new { invoiceId = dto.InvoiceId }, null);
        }

        [HttpGet("{invoiceId:int}")]
        public async Task<IActionResult> GetById(int invoiceId)
        {
            var invoice = await _service.GetByLegalFeeInvoiceIdAsync(invoiceId);
            if (invoice is null)
                return NotFound();

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _service.GetAllLegalFeeInvoiceAsync();
            return Ok(invoices);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LegalFeeInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateLegalFeeInvoiceAsync(dto);
            if (!updated)
                return NotFound($"Invoice ID {dto.InvoiceId} not found.");

            return NoContent();
        }

        [HttpDelete("{invoiceId:int}")]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            var deleted = await _service.DeleteLegalFeeInvoiceAsync(invoiceId);
            if (!deleted)
                return NotFound($"Invoice ID {invoiceId} not found.");

            return NoContent();
        }
    }
}