using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Property;
using PropertyManagementAPI.Domain.DTOs;

[ApiController]
[Route("api/properties/{propertyId}/photos")]
public class PropertyPhotosController : ControllerBase
{
    private readonly IPropertyPhotosService _photoService;
    private readonly IPropertyService _propertyService;
    private readonly ILogger<PropertyPhotosController> _logger;

    public PropertyPhotosController(
        IPropertyPhotosService photoService,
        IPropertyService propertyService,
        ILogger<PropertyPhotosController> logger)
    {
        _photoService = photoService;
        _propertyService = propertyService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> UploadPhoto(int propertyId, [FromBody] PropertyPhotosDto photoDto)
    {
        if (photoDto == null || propertyId != photoDto.PropertyId)
        {
            _logger.LogWarning("UploadPhoto: Invalid photo data for property {PropertyId}.", propertyId);
            return BadRequest("Invalid photo data.");
        }

        var property = await _propertyService.GetPropertyByIdAsync(propertyId);
        if (property == null)
        {
            _logger.LogWarning("UploadPhoto: Property ID {PropertyId} does not exist.", propertyId);
            return NotFound($"Property ID {propertyId} not found.");
        }

        var result = await _photoService.UploadPhotoAsync(photoDto);
        _logger.LogInformation("UploadPhoto: Photo uploaded for property {PropertyId} with ID {PhotoId}.", propertyId, result.PhotoId);
        return CreatedAtAction(nameof(GetPhotos), new { propertyId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPhotos(int propertyId)
    {
        var property = await _propertyService.GetPropertyByIdAsync(propertyId);
        if (property == null)
        {
            _logger.LogWarning("GetPhotos: Property ID {PropertyId} does not exist.", propertyId);
            return NotFound($"Property ID {propertyId} not found.");
        }

        var photos = await _photoService.GetPhotosForPropertyAsync(propertyId);
        _logger.LogInformation("GetPhotos: Retrieved {Count} photos for property {PropertyId}.", photos.Count(), propertyId);
        return Ok(photos);
    }

    [HttpDelete("{photoId}")]
    public async Task<IActionResult> DeletePhoto(int propertyId, int photoId)
    {
        var property = await _propertyService.GetPropertyByIdAsync(propertyId);
        if (property == null)
        {
            _logger.LogWarning("DeletePhoto: Property ID {PropertyId} does not exist.", propertyId);
            return NotFound($"Property ID {propertyId} not found.");
        }

        var deleted = await _photoService.RemovePhotoAsync(photoId);
        if (!deleted)
        {
            _logger.LogWarning("DeletePhoto: Photo ID {PhotoId} not found for property {PropertyId}.", photoId, propertyId);
            return NotFound("Photo not found.");
        }

        _logger.LogInformation("DeletePhoto: Photo ID {PhotoId} deleted for property {PropertyId}.", photoId, propertyId);
        return NoContent();
    }
}