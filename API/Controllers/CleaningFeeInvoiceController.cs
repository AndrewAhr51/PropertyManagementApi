using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.InvoiceOLD;
using PropertyManagementAPI.Domain.DTOs.InvoiceOLD;
using PropertyManagementAPI.Domain.Entities.InvoiceOLD;

namespace PropertyManagementAPI.API.Controllers.Invoices
{
    [ApiController]
    [Route("api/[controller]")]
    public class CleaningFeeInvoiceController : ControllerBase
    {
        private readonly ICleaningFeeInvoiceService _service;
        private readonly ILogger<CleaningFeeInvoiceController> _logger;

        public CleaningFeeInvoiceController(
            ICleaningFeeInvoiceService service,
            ILogger<CleaningFeeInvoiceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CleaningFeeInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateCleaningFeeInvoiceAsync(dto);
            if (!created)
                return StatusCode(500, "Failed to create cleaning fee invoice.");

            return CreatedAtAction(nameof(GetById), new { invoiceId = dto.InvoiceId }, null);
        }

        [HttpGet("{invoiceId:int}")]
        public async Task<IActionResult> GetById(int invoiceId)
        {
            var invoice = await _service.GetCleaningFeeInvoiceByIdAsync(invoiceId);
            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _service.GetAllCleaningFeeInvoiceAsync();
            return Ok(invoices);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CleaningFeeInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateCleaningFeeInvoiceAsync(dto);
            if (!updated)
                return NotFound($"Invoice with ID {dto.InvoiceId} not found.");

            return NoContent();
        }

        [HttpDelete("{invoiceId:int}")]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            var deleted = await _service.DeleteAsync(invoiceId);
            if (!deleted)
                return NotFound($"Invoice with ID {invoiceId} not found.");

            return NoContent();
        }
    }
}