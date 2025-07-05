
#if DEBUG
using Intuit.Ipp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Application.Configuration;
using PropertyManagementAPI.Application.Services.Accounting.Quickbooks;
using PropertyManagementAPI.Application.Services.Auth;
using PropertyManagementAPI.Application.Services.Tenants;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using Stripe;
using Customer = Intuit.Ipp.Data.Customer;
using PropertyManagementAPI.Application.Services.Invoices;


namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/test/quickbooks")]
    public class QuickBooksTestController : ControllerBase
    {
        private readonly QuickBooksTokenManager _tokenManager;
        private readonly QuickBooksInvoiceService _qbInvoiceService;
        private readonly ITenantService _tenantService;
        private readonly IInvoiceService _invoiceService;

        public QuickBooksTestController(QuickBooksTokenManager tokenManager, QuickBooksInvoiceService qbinvoiceService, ITenantService tenantService, IInvoiceService invoiceService)
        {
            _tokenManager = tokenManager;
            _qbInvoiceService = qbinvoiceService;
            _tenantService = tenantService;
            _invoiceService = invoiceService;
        }

        [HttpGet("generate-token")]
        public async Task<IActionResult> GenerateToken()
        {
            var token = await _tokenManager.GetTokenAsync();
            return Ok(new { access_token = token });
        }

        [HttpPost("sync-invoice-quickbooks")]
        public async Task<IActionResult> SyncInvoiceToQuickBooksAsync([FromQuery] int tenantId, [FromBody] int invoiceId)
        {
            var tenant = await _tenantService.GetTenantByIdAsync(tenantId);
            if (tenant == null || string.IsNullOrWhiteSpace(tenant.RealmId))
                return BadRequest("Tenant or realmId not found.");

            var localInvoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
            if (localInvoice == null)
                return NotFound("Invoice not found.");

            var customer = new Customer
            {
                Id = localInvoice.TenantId.ToString(),
                DisplayName = localInvoice.TenantName,
                PrimaryEmailAddr = new EmailAddress { Address = localInvoice.Email }
            };

            var lines = localInvoice.LineItems.Select(item =>
            {
                var detail = new SalesItemLineDetail
                {
                    ItemRef = new ReferenceType
                    {
                        name = item.LineItemTypeName ?? "Service",
                        Value = "123" // Replace with actual Item.Id
                    }
                };

                return new Line
                {
                    DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                    Amount = item.Amount,
                    Description = item.Description ?? "Invoice line item",
                    AnyIntuitObject = detail // 👈 this resolves polymorphic binding
                };
            }).ToList();

            // Assuming the first line's ItemRef.Value is used as the itemId
            var itemId = lines.FirstOrDefault()?.AnyIntuitObject is SalesItemLineDetail detail ? detail.ItemRef.Value : "123";

            var qbInvoice = await _qbInvoiceService.CreateInvoiceAsync(
                tenant.RealmId,
                customer,
                localInvoice.LineItems.Sum(li => li.Amount), 
                itemId
            );

            return Ok(qbInvoice);
        }
    }
}
#endif