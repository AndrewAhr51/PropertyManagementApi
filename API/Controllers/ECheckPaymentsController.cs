using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments.Banking;
using PropertyManagementAPI.Domain.DTOs.Banking;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/echeck")]
    public class ECheckPaymentsController : ControllerBase
    {
        private readonly IECheckPaymentService _service;

        public ECheckPaymentsController(IECheckPaymentService service)
        {
            _service = service;
        }

        // POST: api/echeck/{tenantId}/accounts
        [HttpPost("{tenantId}/accounts")]
        public async Task<IActionResult> AddBankAccount(int tenantId, [FromBody] AddBankAccountDto dto)
        {
            var result = await _service.AddBankAccountAsync(tenantId, dto.BankName, dto.Last4, dto.StripeBankAccountId);
            return Ok(result);
        }

        // POST: api/echeck/{tenantId}/pay
        [HttpPost("{tenantId}/pay")]
        public async Task<IActionResult> SubmitPayment(int tenantId, [FromBody] SubmitPaymentDto dto)
        {
            var result = await _service.SubmitPaymentAsync(tenantId, dto.Amount, dto.StripePaymentIntentId);
            return Ok(result);
        }

        // GET: api/echeck/{tenantId}/transactions
        [HttpGet("{tenantId}/transactions")]
        public async Task<IActionResult> GetTransactions(int tenantId)
        {
            var transactions = await _service.GetTenantTransactionsAsync(tenantId);
            return Ok(transactions);
        }
    }
}
