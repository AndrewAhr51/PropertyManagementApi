using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using PropertyManagementAPI.Domain.DTOs.Banking;
using PropertyManagementAPI.Application.Services.Payments.Banking;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/banking")]
    public class BankingController : ControllerBase
    {
        private readonly IECheckPaymentService _paymentService;
        private readonly ILogger<BankingController> _logger;

        public BankingController(IECheckPaymentService paymentService, ILogger<BankingController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a verified bank account for the specified tenant.
        /// </summary>
        [HttpPost("{tenantId}/bank-account")]
        [ProducesResponseType(typeof(BankAccount), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddBankAccount(int tenantId, [FromBody] AddBankAccountDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _paymentService.AddBankAccountAsync(tenantId, dto.BankName, dto.Last4, dto.StripeBankAccountId);
            _logger.LogInformation("Bank account added for TenantId {TenantId}", tenantId);
            return Ok(result);
        }

        /// <summary>
        /// Submits a payment via ACH.
        /// </summary>
        [HttpPost("{tenantId}/payment")]
        [ProducesResponseType(typeof(PaymentTransactions), StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitPayment(int tenantId, [FromBody] SubmitPaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _paymentService.SubmitPaymentAsync(tenantId, dto.Amount, dto.StripePaymentIntentId);
            _logger.LogInformation("Payment submitted for TenantId {TenantId}", tenantId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves all payment transactions for a tenant.
        /// </summary>
        [HttpGet("{tenantId}/transactions")]
        [ProducesResponseType(typeof(IEnumerable<PaymentTransactions>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTransactions(int tenantId)
        {
            var transactions = await _paymentService.GetTenantTransactionsAsync(tenantId);
            return Ok(transactions);
        }
    }
}