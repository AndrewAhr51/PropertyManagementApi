using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.InvoiceOLD;
using PropertyManagementAPI.Domain.DTOs.InvoiceOLD;
using PropertyManagementAPI.Domain.Entities.InvoiceOLD;

namespace PropertyManagementAPI.API.Controllers.Invoices
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyTaxInvoiceController : ControllerBase
    {
        private readonly IPropertyTaxInvoiceService _service;
        private readonly ILogger<PropertyTaxInvoiceController> _logger;

        public PropertyTaxInvoiceController(
            IPropertyTaxInvoiceService service,
            ILogger<PropertyTaxInvoiceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PropertyTaxInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _service.CreatePropertyTaxInvoiceAsync(dto);
            if (!success)
                return StatusCode(500, "Failed to create Property Tax Invoice.");

            return CreatedAtAction(nameof(GetById), new { invoiceId = dto.InvoiceId }, null);
        }

        [HttpGet("{invoiceId:int}")]
        public async Task<IActionResult> GetById(int invoiceId)
        {
            var invoice = await _service.GetByPropertyTaxInvoiceIdAsync(invoiceId);
            if (invoice is null)
                return NotFound();

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _service.GetAllPropertyTaxInvoiceAsync();
            return Ok(invoices);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PropertyTaxInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdatePropertyTaxInvoiceAsync(dto);
            if (!updated)
                return NotFound($"Invoice ID {dto.InvoiceId} not found.");

            return NoContent();
        }

        [HttpDelete("{invoiceId:int}")]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            var deleted = await _service.DeletePropertyTaxInvoiceAsync(invoiceId);
            if (!deleted)
                return NotFound($"Invoice ID {invoiceId} not found.");

            return NoContent();
        }
    }
}