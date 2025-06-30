using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs.Property;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/leases")]
    public class LeaseController : ControllerBase
    {
        private readonly ILeaseService _service;
        private readonly ILogger<LeaseController> _logger;

        public LeaseController(ILeaseService service, ILogger<LeaseController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateLease([FromBody] LeaseDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Create: Received null LeaseDto.");
                return BadRequest("Invalid lease data.");
            }

            var created = await _service.CreateLeaseAsync(dto);
            _logger.LogInformation("Create: Lease created with ID {Id}.", created.LeaseId);
            return CreatedAtAction(nameof(GetLeaseById), new { leaseId = created.LeaseId }, created);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllLeases()
        {
            var leases = await _service.GetAllLeaseAsync();
            _logger.LogInformation("GetAll: Retrieved {Count} leases.", leases.Count());
            return Ok(leases);
        }

        [HttpGet("by-id/{leaseId}")]
        public async Task<IActionResult> GetLeaseById(int leaseId)
        {
            var lease = await _service.GetLeaseByIdAsync(leaseId);
            if (lease == null)
            {
                _logger.LogWarning("GetById: Lease ID {Id} not found.", leaseId);
                return NotFound();
            }

            return Ok(lease);
        }

        [HttpGet("by-owner/{ownerId}")]
        public async Task<IActionResult> GetAllLeasesByOwnerIdAsync(int ownerId)
        {
            var lease = await _service.GetAllLeasesByOwnerIdAsync(ownerId);
            if (lease == null)
            {
                _logger.LogWarning("GetLeaseByOwnerIdAsync: owner ID {Id} not found.", ownerId);
                return NotFound();
            }

            return Ok(lease);
        }

        [HttpGet("by-tenant/{tenantId}")]
        public async Task<IActionResult> GetLeaseByTenantIdAsync(int tenantId)
        {
            var lease = await _service.GetLeaseByTenantIdAsync(tenantId);
            if (lease == null)
            {
                _logger.LogWarning("GetLeaseByTenantIdAsync: tenant {Id} not found.", tenantId);
                return NotFound();
            }

            return Ok(lease);
        }

        [HttpPut("updateByLeaseid")]
        public async Task<IActionResult> UpdateLease([FromBody] LeaseUpdateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Update: Received null LeaseDto for ID {Id}.", dto.LeaseId);
                return BadRequest("Invalid lease data.");
            }

            var updated = await _service.UpdateLeaseAsync(dto);
            if (!updated)
            {
                _logger.LogWarning("Update: Lease ID {Id} not found or inactive.", dto.LeaseId);
                return NotFound();
            }

            _logger.LogInformation("Update: Lease ID {Id} updated.", dto.LeaseId);
            return NoContent();
        }

        [HttpDelete("delete/{leaseId}")]
        public async Task<IActionResult> DeleteLease(int leaseId)
        {
            var deleted = await _service.DeleteLeaseAsync(leaseId);
            if (!deleted)
            {
                _logger.LogWarning("Delete: Lease ID {Id} not found or already inactive.", leaseId);
                return NotFound();
            }

            _logger.LogInformation("Delete: Lease ID {Id} soft-deleted.", leaseId);
            return NoContent();
        }
    }
}