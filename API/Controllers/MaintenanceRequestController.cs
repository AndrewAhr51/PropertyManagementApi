using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs.Maintenance;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/maintenance-requests")]
    public class MaintenanceRequestController : ControllerBase
    {
        private readonly IMaintenanceRequestService _service;
        private readonly ILogger<MaintenanceRequestController> _logger;

        public MaintenanceRequestController(IMaintenanceRequestService service, ILogger<MaintenanceRequestController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MaintenanceRequestDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Create: Received null MaintenanceRequestDto.");
                return BadRequest("Invalid maintenance request data.");
            }

            var created = await _service.CreateAsync(dto);
            _logger.LogInformation("Create: Maintenance request created with ID {Id}.", created.RequestId);
            return CreatedAtAction(nameof(GetById), new { requestId = created.RequestId }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _service.GetAllAsync();
            _logger.LogInformation("GetAll: Retrieved {Count} maintenance requests.", requests.Count());
            return Ok(requests);
        }

        [HttpGet("{requestId}")]
        public async Task<IActionResult> GetById(int requestId)
        {
            var request = await _service.GetByIdAsync(requestId);
            if (request == null)
            {
                _logger.LogWarning("GetById: Maintenance request ID {Id} not found.", requestId);
                return NotFound();
            }

            return Ok(request);
        }

        [HttpPut("{requestId}")]
        public async Task<IActionResult> Update(int requestId, [FromBody] MaintenanceRequestDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Update: Received null MaintenanceRequestDto for ID {Id}.", requestId);
                return BadRequest("Invalid maintenance request data.");
            }

            var updated = await _service.UpdateAsync(requestId, dto);
            if (!updated)
            {
                _logger.LogWarning("Update: Maintenance request ID {Id} not found.", requestId);
                return NotFound();
            }

            _logger.LogInformation("Update: Maintenance request ID {Id} updated.", requestId);
            return NoContent();
        }

        [HttpDelete("{requestId}")]
        public async Task<IActionResult> Delete(int requestId)
        {
            var deleted = await _service.DeleteAsync(requestId);
            if (!deleted)
            {
                _logger.LogWarning("Delete: Maintenance request ID {Id} not found.", requestId);
                return NotFound();
            }

            _logger.LogInformation("Delete: Maintenance request ID {Id} deleted.", requestId);
            return NoContent();
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var requests = await _service.GetByUserIdAsync(userId);
            if (!requests.Any())
            {
                _logger.LogWarning("GetByUserId: No requests found for user ID {UserId}.", userId);
                return NotFound();
            }

            return Ok(requests);
        }

        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetByPropertyId(int propertyId)
        {
            var requests = await _service.GetByPropertyIdAsync(propertyId);
            if (!requests.Any())
            {
                _logger.LogWarning("GetByPropertyId: No requests found for property ID {PropertyId}.", propertyId);
                return NotFound();
            }

            return Ok(requests);
        }
    }
}