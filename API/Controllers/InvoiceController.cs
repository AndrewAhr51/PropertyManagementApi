using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Domain.DTOs;

[ApiController]
[Route("api/[controller]")]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet("{invoiceId}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int invoiceId)
    {
        var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
        if (invoice == null) return NotFound();

        return Ok(invoice);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAllInvoices()
    {
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        return Ok(invoices);
    }

    [HttpGet("tenant/{tenantId}")]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoicesByTenantId(int tenantId)
    {
        var invoices = await _invoiceService.GetInvoicesByTenantIdAsync(tenantId);
        return Ok(invoices);
    }

    [HttpPost]
    public async Task<ActionResult> CreateInvoice([FromBody] InvoiceDto invoiceDto)
    {
        if (invoiceDto == null) return BadRequest();

        var success = await _invoiceService.CreateInvoiceAsync(invoiceDto);
        if (!success) return StatusCode(500, "Error creating invoice");

        return CreatedAtAction(nameof(GetInvoiceById), new { invoiceId = invoiceDto.InvoiceId }, invoiceDto);
    }

    [HttpPut("{invoiceId}")]
    public async Task<ActionResult> UpdateInvoice(int invoiceId, [FromBody] InvoiceDto invoiceDto)
    {
        if (invoiceDto == null || invoiceId != invoiceDto.InvoiceId) return BadRequest();

        var success = await _invoiceService.UpdateInvoiceAsync(invoiceDto);
        if (!success) return StatusCode(500, "Error updating invoice");

        return NoContent();
    }

    [HttpDelete("{invoiceId}")]
    public async Task<ActionResult> DeleteInvoice(int invoiceId)
    {
        var success = await _invoiceService.DeleteInvoiceAsync(invoiceId);
        if (!success) return NotFound();

        return NoContent();
    }
}