using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/payment-methods")]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentMethodsService _service;
        private readonly ILogger<PaymentMethodsController> _logger;

        public PaymentMethodsController(IPaymentMethodsService service, ILogger<PaymentMethodsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentMethodsDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Create: Received null PaymentMethodDto.");
                return BadRequest("Invalid payment method data.");
            }

            var created = await _service.CreateAsync(dto);
            _logger.LogInformation("Create: Payment method '{MethodName}' created with ID {Id}.", created.MethodName, created.PaymentMethodId);
            return CreatedAtAction(nameof(GetById), new { id = created.PaymentMethodId }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var methods = await _service.GetAllAsync();
            _logger.LogInformation("GetAll: Retrieved {Count} payment methods.", methods.Count());
            return Ok(methods);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var method = await _service.GetByIdAsync(id);
            if (method == null)
            {
                _logger.LogWarning("GetById: Payment method ID {Id} not found.", id);
                return NotFound();
            }

            return Ok(method);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentMethodsDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Update: Received null PaymentMethodDto for ID {Id}.", id);
                return BadRequest("Invalid payment method data.");
            }

            var updated = await _service.UpdateAsync(id, dto);
            if (!updated)
            {
                _logger.LogWarning("Update: Payment method ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Update: Payment method ID {Id} updated.", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Delete: Payment method ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Delete: Payment method ID {Id} deleted.", id);
            return NoContent();
        }
    }
}