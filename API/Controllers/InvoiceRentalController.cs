using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceRentalController : ControllerBase
    {
        private readonly IInvoiceRentalService _invoiceRentalService;

        public InvoiceRentalController(IInvoiceRentalService invoiceRentalService)
        {
            _invoiceRentalService = invoiceRentalService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RentInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var saved = await _invoiceRentalService.CreateInvoiceRentalAsync(dto);

            if (!saved)
                return StatusCode(StatusCodes.Status500InternalServerError, $"Failed to create rental invoice for {dto.PropertyId}.");

            return CreatedAtAction(nameof(GetById), new { invoiceId = dto.InvoiceId }, null);
        }

        [HttpGet("{invoiceId:int}")]
        public async Task<ActionResult<RentInvoice>> GetById(int invoiceId)
        {
            var invoice = await _invoiceRentalService.GetInvoiceRentalByIdAsync(invoiceId);
            return invoice is not null ? Ok(invoice) : NotFound();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentInvoice>>> GetAll()
        {
            var invoices = await _invoiceRentalService.GetAllInvoicesRentalsAsync();
            return Ok(invoices);
        }

        [HttpGet("by-month-year")]
        public async Task<ActionResult<IEnumerable<RentInvoice>>> GetByMonthYear([FromQuery] int month, [FromQuery] int year)
        {
            var invoices = await _invoiceRentalService.GetInvoicesRentalsByMonthYearAsync(month, year);
            return Ok(invoices);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RentInvoiceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _invoiceRentalService.UpdateInvoiceRentalAsync(dto);
            return NoContent();
        }

        [HttpDelete("{invoiceId:int}")]
        public async Task<IActionResult> Delete(int invoiceId)
        {
            await _invoiceRentalService.DeleteInvoiceRentalAsync(invoiceId);
            return NoContent();
        }
    }
}