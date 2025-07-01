using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs.Users;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/tenants")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _service;
        private readonly ILogger<TenantController> _logger;

        public TenantController(ITenantService service, ILogger<TenantController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] TenantDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Create: Received null TenantDto.");
                return BadRequest("Invalid tenant data.");
            }

            var created = await _service.CreateTenantsAsync(dto);
            _logger.LogInformation("Create: Tenant created with ID {Id}.", created.TenantId);
            return CreatedAtAction(nameof(GetTenantById), new { tenantId = created.TenantId }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTenants()
        {
            var tenants = await _service.GetAllTenantsAsync();
            _logger.LogInformation("GetAll: Retrieved {Count} tenants.", tenants.Count());
            return Ok(tenants);
        }

        [HttpGet("{tenantId}")]
        public async Task<IActionResult> GetTenantById(int tenantId)
        {
            var tenant = await _service.GetTenantByIdAsync(tenantId);
            if (tenant == null)
            {
                _logger.LogWarning("GetById: Tenant ID {Id} not found.", tenantId);
                return NotFound();
            }

            return Ok(tenant);
        }

        [HttpPut("{tenantId}")]
        public async Task<IActionResult> UpdateTenant(int tenantId, [FromBody] TenantDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Update: Received null TenantDto for ID {Id}.", tenantId);
                return BadRequest("Invalid tenant data.");
            }

            var updated = await _service.UpdateTenantAsync(tenantId, dto);
            if (!updated)
            {
                _logger.LogWarning("Update: Tenant ID {Id} not found.", tenantId);
                return NotFound();
            }

            _logger.LogInformation("Update: Tenant ID {Id} updated.", tenantId);
            return NoContent();
        }

        [HttpDelete("{tenantId}")]
        public async Task<IActionResult> SetActivateTenant(int tenantId)
        {
            var deleted = await _service.SetActivateTenant(tenantId);
            if (!deleted)
            {
                _logger.LogWarning("Delete: Tenant ID {Id} not found.", tenantId);
                return NotFound();
            }

            _logger.LogInformation("Delete: Tenant ID {Id} deleted.", tenantId);
            return NoContent();
        }
    }
}