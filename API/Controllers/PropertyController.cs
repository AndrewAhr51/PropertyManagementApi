using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services.Property;
using PropertyManagementAPI.Domain.DTOs.Property;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/properties")]
    [Tags("Property")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly ILogger<PropertyController> _logger;

        public PropertyController(IPropertyService propertyService, ILogger<PropertyController> logger)
        {
            _propertyService = propertyService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddProperty([FromBody] PropertyDto propertyDto)
        {
            if (propertyDto == null || propertyDto.OwnerId <= 0)
            {
                _logger.LogWarning("Invalid property data received.");
                return BadRequest("Invalid property data.");
            }

            try
            {
                var result = await _propertyService.AddPropertyAsync(propertyDto);
                return CreatedAtAction(nameof(GetPropertyById), new { propertyId = result.PropertyId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding property.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetPropertyById(int propertyId)
        {
            var property = await _propertyService.GetPropertyByIdAsync(propertyId);
            if (property == null)
            {
                _logger.LogWarning($"Property with ID {propertyId} not found.");
                return NotFound("Property not found.");
            }

            return Ok(property);
        }

        [HttpGet("by-owner/{ownerId}")]
        public async Task<IActionResult> GetPropertiesByOwnerId(int ownerId)
        {
            var properties = await _propertyService.GetPropertiesByOwnerIdAsync(ownerId);
            return Ok(properties);
        }

        [HttpPut("{propertyId}")]
        public async Task<IActionResult> UpdateProperty(int propertyId, [FromBody] PropertyDto propertyDto)
        {
            var updated = await _propertyService.UpdatePropertyAsync(propertyDto);
            if (updated == null)
            {
                _logger.LogWarning($"Property with ID {propertyId} not found for update.");
                return NotFound("Property not found.");
            }

            return Ok(updated);
        }

        [HttpPut("{propertyId}/setactivate")]
        public async Task<IActionResult> SetActivateProperty(int propertyId)
        {
            var success = await _propertyService.SetActivatePropertyAsync(propertyId);
            if (!success)
            {
                _logger.LogWarning($"Property with ID {propertyId} not found or already inactive.");
                return NotFound("Property not found or already inactive.");
            }

            return Ok("Property inactivated successfully.");
        }
    }
}