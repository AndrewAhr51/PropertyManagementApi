using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.API.Controllers.Invoices
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityDepositInvoiceController : ControllerBase
    {
        private readonly ISecurityDepositInvoiceService _service;
        private readonly ILogger<SecurityDepositInvoiceController> _logger;

        public SecurityDepositInvoiceController(
            ISecurityDepositInvoiceService service,
            ILogger<SecurityDepositInvoiceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SecurityDepositInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateSecurityDepositInvoiceAsync(dto);
            if (!result)
                return StatusCode(500, "Failed to create security deposit invoice.");

            return CreatedAtAction(nameof(GetById), new { invoiceId = dto.InvoiceId }, null);
        }

        [HttpGet("{invoiceId:int}")]
        public async Task<IActionResult> GetById(int invoiceId)
        {
            var invoice = await _service.GetSecurityDepositInvoiceByIdAsync(invoiceId);
            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _service.GetAllSecurityDepositInvoiceAsync();
            return Ok(invoices);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SecurityDepositInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateSecurityDepositInvoiceAsync(dto);
            if (!result)
                return NotFound($"Invoice with ID {dto.InvoiceId} not found.");

            return NoContent();
        }

        [HttpDelete("{invoiceId:int}")]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            var result = await _service.DeleteSecurityDepositInvoiceAsync(invoiceId);
            if (!result)
                return NotFound($"Invoice with ID {invoiceId} not found.");

            return NoContent();
        }
    }
}