using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Owners;
using PropertyManagementAPI.Domain.DTOs.Users;

[ApiController]
[Route("api/owners")]
public class OwnerController : ControllerBase
{
    private readonly IOwnersService _ownersService;
    private readonly ILogger<OwnerController> _logger;

    public OwnerController(IOwnersService ownersService, ILogger<OwnerController> logger)
    {
        _ownersService = ownersService;
        _logger = logger;
    }

    // ✅ Add a new owner
    [HttpPost]
    public async Task<IActionResult> AddOwner([FromBody] OwnerDto ownerDto)
    {  
        var ownerExists = await _ownersService.GetOwnerByIdAsync(ownerDto.OwnerId);
        if (ownerExists != null)
        {
            _logger.LogWarning($"Owner with UserId {ownerDto.OwnerId} already exists.");
            return Conflict("Owner already exists.");
        }

        var owner = await _ownersService.AddOwnerAsync(ownerDto);
        _logger.LogInformation($"Owner {ownerDto.FirstName} {ownerDto.LastName} added successfully.");
        return CreatedAtAction(nameof(GetOwnerById), new { ownerId = owner.OwnerId }, owner);
    }

    // ✅ Get all owners
    [HttpGet]
    public async Task<IActionResult> GetAllOwners()
    {
        var owners = await _ownersService.GetAllOwnersAsync();
        if (owners == null || !owners.Any())
        {
            _logger.LogWarning("No owners found.");
            return NotFound("No owners found.");
        }

        return Ok(owners);
    }

    // ✅ Get owner by ID
    [HttpGet("{ownerId}")]
    public async Task<IActionResult> GetOwnerById(int ownerId)
    {
        var owner = await _ownersService.GetOwnerByIdAsync(ownerId);
        if (owner == null)
        {
            _logger.LogWarning($"Owner with ID {ownerId} not found.");
            return NotFound("Owner not found.");
        }

        return Ok(owner);
    }

    // ✅ Get owner by username
    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetOwnerByUserName(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            _logger.LogWarning("Invalid username provided.");
            return BadRequest("Username cannot be empty.");
        }

        var owner = await _ownersService.GetOwnerByUserNameAsync(username);
        if (owner == null)
        {
            _logger.LogWarning($"Owner with username '{username}' not found.");
            return NotFound("Owner not found.");
        }

        return Ok(owner);
    }

    // ✅ Update owner details
    [HttpPut("{ownerId}")]
    public async Task<IActionResult> UpdateOwner(int ownerId, [FromBody] OwnerDto ownerDto)
    {
        if (ownerDto == null || ownerId <= 0)
        {
            _logger.LogWarning("Invalid owner update request.");
            return BadRequest("Invalid owner data.");
        }

        var save = await _ownersService.UpdateOwnerAsync(ownerDto);
        
        _logger.LogInformation($"Owner {ownerDto.FirstName} {ownerDto.LastName} updated successfully.");
        return Ok(save);
    }

    // ✅ Delete owner
    [HttpDelete("{ownerId}")]
    public async Task<IActionResult> DeleteOwner(int ownerId)
    {
        var ownerExists = await _ownersService.GetOwnerByIdAsync(ownerId);
        if (ownerExists == null)
        {
            _logger.LogWarning($"Owner with ID {ownerId} not found.");
            return NotFound("Owner not found.");
        }

        await _ownersService.DeleteOwnerAsync(ownerId);
        _logger.LogInformation($"Owner with ID {ownerId} deleted successfully.");
        return NoContent();
    }

    [HttpPut("{ownerId}/setactivate")]
    public async Task<IActionResult> SetActivateOwner(int ownerId)
    {
        var success = await _ownersService.SetActivateOwnerAsync(ownerId);
        if (!success)
        {
            _logger.LogWarning($"Owner with ID {ownerId} not found or already inactive.");
            return NotFound("Owner not found or already inactive.");
        }

        _logger.LogInformation($"Owner with ID {ownerId} has been inactivated.");
        return Ok("Owner inactivated successfully.");
    }

}