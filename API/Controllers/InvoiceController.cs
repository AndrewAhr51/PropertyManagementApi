using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoices.Mappers;
using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/invoices")]
    [Tags("Invoices")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        // 🔹 Get all invoices
        [HttpGet("get-all-invoices")]
        public async Task<ActionResult<List<InvoiceDto>>> GetAllInvoices()
        {
            try
            {
                _logger.LogInformation("API Request: Retrieving all invoices");
                var invoices = await _invoiceService.GetAllInvoicesAsync();
                _logger.LogInformation("Audit: Retrieved {InvoiceCount} invoice(s)", invoices.Count);
                return Ok(invoices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices");
                return StatusCode(500, "Failed to retrieve invoices");
            }
        }

        // 🔹 Get all invoices for a tenant
        [HttpGet("get-all-invoices-by-tenantid/{tenantId}")]
        public async Task<ActionResult<OpenInvoiceByTenantDto>> GetAllInvoicesByTenantId(int tenantId)
        {
            try
            {
                _logger.LogInformation("API Request: Retrieving all invoices");
                var invoicesListByTenant = await _invoiceService.GetAllInvoicesByTenantIdAsync(tenantId);
                _logger.LogInformation("Audit: Retrieved {InvoiceCount} invoice(s)", invoicesListByTenant);
                return Ok(invoicesListByTenant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenants invoices");
                return StatusCode(500, "Failed to retrieve teanants invoices");
            }
        }

        // 🔹 Get invoices for a tenant/invoice id
        [HttpGet("get-all-invoices-by-tenantid-invoiceid/{tenantId}/{invoiceId}")]
        public async Task<ActionResult<OpenInvoiceByTenantDto>> GetInvoiceByTenantIdandInvoiceId(int tenantId, int invoiceId)
        {
            try
            {
                _logger.LogInformation("API Request: Retrieving invoice for tenant with invoiceid");
                var invoicesListByTenant = await _invoiceService.GetInvoiceByTenantIdandInvoiceIdIdAsync(tenantId, invoiceId);
                _logger.LogInformation("Audit: Retrieved {InvoiceCount} invoice(s)", invoicesListByTenant);
                return Ok(invoicesListByTenant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenants invoices");
                return StatusCode(500, "Failed to retrieve teanants invoices");
            }
        }

        // 🔹 Get invoice by ID
        [HttpGet("get-invoice-by-invoiceid/{invoiceId}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoice(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                {
                    _logger.LogWarning("Audit: InvoiceId {InvoiceId} not found", invoiceId);
                    return NotFound();
                }

                _logger.LogInformation("Audit: Retrieved invoice for InvoiceId {InvoiceId}", invoiceId);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice for InvoiceId {InvoiceId}", invoiceId);
                return StatusCode(500, "Failed to retrieve invoice");
            }
        }

        // 🔹 Create invoice
        [HttpPost("create-invoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Audit: CreateInvoiceDto is null");
                    return BadRequest("Invalid invoice data.");
                }

                if (dto.PropertyId <= 0)
                {
                    _logger.LogWarning("Audit: Invalid PropertyId {PropertyId} in CreateInvoiceDto", dto.PropertyId);
                    return BadRequest("Invalid PropertyId.");
                }

                _logger.LogInformation("API Request: Creating invoice for PropertyId {PropertyId}", dto.PropertyId);
                var success = await _invoiceService.CreateInvoiceAsync(dto);

                if (success)
                {
                    _logger.LogInformation("Audit: Invoice created successfully for PropertyId {PropertyId}", dto.PropertyId);
                    return Ok();
                }

                _logger.LogWarning("Audit: Invoice creation failed for PropertyId {PropertyId}", dto.PropertyId);
                return BadRequest("Invoice creation failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating invoice for PropertyId {PropertyId}", dto.PropertyId);
                return StatusCode(500, "Failed to create invoice");
            }
        }

        // 🔹 Update invoice
        [HttpPut("update-invoice-by-invoiceid/{invoiceId}")]
        public async Task<IActionResult> UpdateInvoice([FromBody] InvoiceDto dto)
        {
            try
            {
                var success = await _invoiceService.UpdateInvoiceAsync(dto);

                if (success)
                {
                    _logger.LogInformation("Audit: Invoice updated successfully for InvoiceId {InvoiceId}", dto.InvoiceId);
                    return NoContent();
                }

                _logger.LogWarning("Audit: Invoice update failed or InvoiceId not found: {InvoiceId}", dto.InvoiceId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice for InvoiceId {InvoiceId}", dto.InvoiceId);
                return StatusCode(500, "Failed to update invoice");
            }
        }

        // 🔹 Delete invoice
        [HttpDelete("delete-invoice/{invoiceId}")]
        public async Task<IActionResult> DeleteInvoice(int invoiceId)
        {
            _logger.LogInformation("API Request: Deleting InvoiceId {InvoiceId}", invoiceId);

            try
            {
                var success = await _invoiceService.DeleteInvoiceAsync(invoiceId);

                if (success)
                {
                    _logger.LogInformation("Audit: InvoiceId {InvoiceId} deleted successfully", invoiceId);
                    return NoContent();
                }

                _logger.LogWarning("Audit: InvoiceId {InvoiceId} deletion failed or not found", invoiceId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting InvoiceId {InvoiceId}", invoiceId);
                return StatusCode(500, "Failed to delete invoice");
            }
        }

        // 🔹 Create line item
        [HttpPost("create-line-items/line-items")]
        public async Task<IActionResult> CreateLineItem([FromBody] CreateInvoiceLineItemDto dto)
        {
           
            _logger.LogInformation("API Request: Creating line item for InvoiceId {InvoiceId}", dto.invoiceId);

            try
            {
                var lineItemId = await _invoiceService.CreateLineItemAsync(dto);
                _logger.LogInformation("Audit: Line item created successfully with LineItemId {LineItemId}", lineItemId);
                return CreatedAtAction(nameof(GetLineItem), new { lineItemId }, lineItemId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating line item for InvoiceId {InvoiceId}", dto.invoiceId);
                return StatusCode(500, "Failed to create line item");
            }
        }

        // 🔹 Get line item
        [HttpGet("get-lineitem-by-lineitemid/{lineItemId}")]
        public async Task<ActionResult<InvoiceLineItemDto>> GetLineItem(int lineItemId)
        {
            try
            {
                var item = await _invoiceService.GetLineItemAsync(lineItemId);
                if (item == null)
                {
                    _logger.LogWarning("Audit: Line item not found for LineItemId {LineItemId}", lineItemId);
                    return NotFound();
                }

                _logger.LogInformation("Audit: Retrieved line item LineItemId {LineItemId}", lineItemId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LineItemId {LineItemId}", lineItemId);
                return StatusCode(500, "Failed to retrieve line item");
            }
        }

        // 🔹 Get all line items for invoice
        [HttpGet("get-all-lineitems-for-invoice/{invoiceId}/line-items")]
        public async Task<ActionResult<List<InvoiceLineItemDto>>> GetLineItemsForInvoice(int invoiceId)
        {
            try
            {
                var items = await _invoiceService.GetLineItemsForInvoiceAsync(invoiceId);
                _logger.LogInformation("Audit: Retrieved {Count} line items for InvoiceId {InvoiceId}", items.Count, invoiceId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving line items for InvoiceId {InvoiceId}", invoiceId);
                return StatusCode(500, "Failed to retrieve line items");
            }
        }

        // 🔹 Update line item
        [HttpPut("update-line-item/{lineItemId}")]
        public async Task<IActionResult> UpdateLineItem(int lineItemId, [FromBody] CreateInvoiceLineItemDto dto)
        {
            try
            {
                var success = await _invoiceService.UpdateLineItemAsync(lineItemId, dto);

                if (success)
                {
                    _logger.LogInformation("Audit: LineItemId {LineItemId} updated successfully", lineItemId);
                    return NoContent();
                }

                _logger.LogWarning("Audit: LineItemId {LineItemId} update failed or not found", lineItemId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating LineItemId {LineItemId}", lineItemId);
                return StatusCode(500, "Failed to update line item");
            }
        }

        // 🔹 Delete line item
        [HttpDelete("delete-line-item/{lineItemId}")]
        public async Task<IActionResult> DeleteLineItem(int lineItemId)
        {
            _logger.LogInformation("API Request: Deleting LineItemId {LineItemId}", lineItemId);

            try
            {
                var success = await _invoiceService.DeleteLineItemAsync(lineItemId);

                if (success)
                {
                    _logger.LogInformation("Audit: LineItemId {LineItemId} deleted successfully", lineItemId);
                    return NoContent();
                }

                _logger.LogWarning("Audit: LineItemId {LineItemId} deletion failed or not found", lineItemId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting LineItemId {LineItemId}", lineItemId);
                return StatusCode(500, "Failed to delete line item");
            }
        }
    }
}