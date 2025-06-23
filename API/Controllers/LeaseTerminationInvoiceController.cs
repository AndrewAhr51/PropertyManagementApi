using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.API.Controllers.Invoices
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaseTerminationInvoiceController : ControllerBase
    {
        private readonly ILeaseTerminationInvoiceService _service;
        private readonly ILogger<LeaseTerminationInvoiceController> _logger;

        public LeaseTerminationInvoiceController(
            ILeaseTerminationInvoiceService service,
            ILogger<LeaseTerminationInvoiceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaseTerminationInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreateLeaseTerminationInvoiceAsync(dto);
            if (!created)
                return StatusCode(500, "Failed to create lease termination invoice.");

            return CreatedAtAction(nameof(GetById), new { invoiceId = dto.InvoiceId }, null);
        }

        [HttpGet("{invoiceId:int}")]
        public async Task<IActionResult> GetById(int invoiceId)
        {
            var invoice = await _service.GetLeaseTerminationInvoiceByIdAsync(invoiceId);
            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _service.GetAllLeaseTerminationInvoiceAsync();
            return Ok(invoices);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LeaseTerminationInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateLeaseTerminationInvoiceAsync(dto);
            if (!updated)
                return NotFound($"Invoice with ID {dto.InvoiceId} not found.");

            return NoContent();
        }

        [HttpDelete("{invoiceId:int}")]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            var deleted = await _service.DeleteLeaseTerminationInvoiceAsync(invoiceId);
            if (!deleted)
                return NotFound($"Invoice with ID {invoiceId} not found.");

            return NoContent();
        }
    }
}