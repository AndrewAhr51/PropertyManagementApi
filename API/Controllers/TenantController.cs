using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Application.Services.Tenants;
using PropertyManagementAPI.Application.Services.Users;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/tenants")]
    [Tags("Tenant")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _service;
        private readonly IUserService _usersService;
        private readonly ILogger<TenantController> _logger;
        private readonly IConfiguration _config;

        public TenantController(ITenantService service, ILogger<TenantController> logger, IConfiguration config, IUserService usersService)
        {
            _service = service;
            _logger = logger;
            _config = config;
            _usersService = usersService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTenant([FromBody] TenantDto dto)
        {
            _logger.LogInformation("CreateTenant: Received request.");

            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("CreateTenant: Received null TenantDto.");
                    return BadRequest("Invalid tenant data.");
                }

                var created = await _service.CreateTenantsAsync(dto);
                _logger.LogInformation("CreateTenant: Tenant created with ID {Id}.", created.TenantId);

                return CreatedAtAction(nameof(GetTenantById), new { tenantId = created.TenantId }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTenant: Error creating tenant.");
                return StatusCode(500, "An error occurred while creating the tenant.");
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllTenants()
        {
            _logger.LogInformation("GetAllTenants: Fetching all tenants.");

            try
            {
                var tenants = await _service.GetAllTenantsAsync();
                _logger.LogInformation("GetAllTenants: Retrieved {Count} tenants.", tenants.Count());

                return Ok(tenants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllTenants: Failed to retrieve tenants.");
                return StatusCode(500, "An error occurred while retrieving tenant records.");
            }
        }

        [HttpGet("by-tenant-id/{tenantId}")]
        public async Task<IActionResult> GetTenantById(int tenantId)
        {
            _logger.LogInformation("GetTenantById: Fetching tenant ID {Id}.", tenantId);

            try
            {
                var tenant = await _service.GetTenantByIdAsync(tenantId);
                if (tenant == null)
                {
                    _logger.LogWarning("GetTenantById: Tenant ID {Id} not found.", tenantId);
                    return NotFound();
                }

                return Ok(tenant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTenantById: Failed to retrieve tenant ID {Id}.", tenantId);
                return StatusCode(500, "An error occurred while retrieving the tenant.");
            }
        }

        [HttpGet("by-property-id/{propertyid}")]
        public async Task<IActionResult> GetTenantByPropertyId(int propertyid)
        {
            _logger.LogInformation("GetTenantByPropertyId: Fetching property ID {Id}.", propertyid);

            try
            {
                var tenant = await _service.GetTenantByPropertyIdAsync(propertyid);
                if (tenant == null)
                {
                    _logger.LogWarning("GetTenantByPropertyId: property ID {Id} not found.", propertyid);
                    return NotFound();
                }

                return Ok(tenant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTenantByPropertyId: Failed to retrieve tenant by property id {propertyid}.", propertyid);
                return StatusCode(500, "An error occurred while retrieving the tenant.");
            }
        }

        [HttpPut("update/{tenantId}")]
        public async Task<IActionResult> UpdateTenant(int tenantId, [FromBody] TenantDto dto)
        {
            _logger.LogInformation("UpdateTenant: Attempting to update tenant ID {Id}.", tenantId);

            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("UpdateTenant: Received null TenantDto.");
                    return BadRequest("Invalid tenant data.");
                }

                var updated = await _service.UpdateTenantAsync(tenantId, dto);
                if (!updated)
                {
                    _logger.LogWarning("UpdateTenant: Tenant ID {Id} not found.", tenantId);
                    return NotFound();
                }

                _logger.LogInformation("UpdateTenant: Tenant ID {Id} updated successfully.", tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateTenant: Error updating tenant ID {Id}.", tenantId);
                return StatusCode(500, "An error occurred while updating the tenant.");
            }
        }

        [HttpPut("{tenantId}/setactivate")]
        public async Task<IActionResult> SetActivateTenant(int tenantId)
        {
            var success = await _usersService.SetActivateUserAsync(tenantId);
            if (!success)
            {
                _logger.LogWarning($"Tenant with ID {tenantId} not found or already inactive.");
                return NotFound("Owner not found or already inactive.");
            }

            _logger.LogInformation($"Tenant with ID {tenantId} has been inactivated.");
            return Ok("Owner inactivated successfully.");
        }
    }
}