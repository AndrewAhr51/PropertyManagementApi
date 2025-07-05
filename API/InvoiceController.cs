using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoices;

namespace PropertyManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // GET: api/Invoice/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
        {
            var invoice = await _invoiceService.GetInvoiceAsync(id);
            if (invoice == null)
                return NotFound();

            return Ok(invoice);
        }

        // GET: api/Invoice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAllInvoices()
        {
            var invoices = await _invoiceService.GetAllInvoicesAsync();
            return Ok(invoices);
        }

        // POST: api/Invoice
        [HttpPost]
        public async Task<ActionResult<int>> CreateInvoice([FromBody] InvoiceDto dto)
        {
            var invoiceId = await _invoiceService.CreateInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetInvoice), new { id = invoiceId }, invoiceId);
        }

        // PUT: api/Invoice/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] InvoiceDto dto)
        {
            if (id != dto.InvoiceId)
                return BadRequest("Invoice ID mismatch");

            var success = await _invoiceService.UpdateInvoiceAsync(dto);
            return success ? NoContent() : NotFound();
        }

        // DELETE: api/Invoice/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var success = await _invoiceService.DeleteInvoiceAsync(id);
            return success ? NoContent() : NotFound();
        }

        // GET: api/Invoice/{id}/line-items
        [HttpGet("{id}/line-items")]
        public async Task<ActionResult<IEnumerable<InvoiceLineItemDto>>> GetLineItems(int id)
        {
            var lineItems = await _invoiceService.GetLineItemsForInvoiceAsync(id);
            return Ok(lineItems);
        }
    }
}