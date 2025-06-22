using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.API.Controllers.Invoices
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceUtilitiesController : ControllerBase
    {
        private readonly IUtilityInvoiceService _utilityInvoiceService;
        private readonly ILogger<InvoiceUtilitiesController> _logger;

        public InvoiceUtilitiesController(IUtilityInvoiceService utilityInvoiceService, ILogger<InvoiceUtilitiesController> logger)
        {
            _utilityInvoiceService = utilityInvoiceService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UtilityInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _utilityInvoiceService.CreateUtilitiesInvoiceAsync(dto);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create utility invoice.");

            return CreatedAtAction(nameof(GetById), new { invoiceId = dto.InvoiceId }, null);
        }

        [HttpGet("{invoiceId:int}")]
        public async Task<IActionResult> GetById(int invoiceId)
        {
            var invoice = await _utilityInvoiceService.GetByIdAsync(invoiceId);
            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _utilityInvoiceService.GetAllAsync();
            return Ok(invoices);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UtilityInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _utilityInvoiceService.UpdateAsync(dto);
            if (!success)
                return NotFound($"No utility invoice found with ID {dto.InvoiceId}.");

            return NoContent();
        }

        [HttpDelete("{invoiceId:int}")]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            var success = await _utilityInvoiceService.DeleteAsync(invoiceId);
            if (!success)
                return NotFound($"No utility invoice found with ID {invoiceId}.");

            return NoContent();
        }
    }
}