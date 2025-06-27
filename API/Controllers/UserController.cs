using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Users;
using PropertyManagementAPI.Domain.DTOs.Users;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
    {
        if (userDto == null)
        {
            _logger.LogWarning("CreateUser: Received null userDto.");
            return BadRequest("Invalid user data.");
        }

        try
        {
            var createdUser = await _userService.CreateUserAsync(userDto);
            _logger.LogInformation("CreateUser: User created with ID {UserId}.", createdUser.UserId);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateUser: Error creating user.");
            return StatusCode(500, "An error occurred while creating the user.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogInformation("GetAllUsers: Fetching all users.");
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        _logger.LogInformation("GetUserById: Looking up user with ID {UserId}.", id);
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("GetUserById: User with ID {UserId} not found.", id);
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
    {
        if (userDto == null)
        {
            _logger.LogWarning("UpdateUser: Received null userDto for ID {UserId}.", id);
            return BadRequest("Invalid user data.");
        }

        var updated = await _userService.UpdateUserAsync(id, userDto);
        if (!updated)
        {
            _logger.LogWarning("UpdateUser: User with ID {UserId} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("UpdateUser: User with ID {UserId} updated successfully.", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("DeleteUser: User with ID {UserId} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("DeleteUser: User with ID {UserId} deleted.", id);
        return NoContent();
    }

    [HttpDelete("username/{username}")]
    public async Task<IActionResult> DeleteUserByUsername(string username)
    {
        var deleted = await _userService.DeleteUserByUsernameAsync(username);
        if (!deleted)
        {
            _logger.LogWarning("DeleteUserByUsername: User '{Username}' not found.", username);
            return NotFound();
        }

        _logger.LogInformation("DeleteUserByUsername: User '{Username}' deleted.", username);
        return NoContent();
    }

    [HttpDelete("email/{email}")]
    public async Task<IActionResult> DeleteUserByEmail(string email)
    {
        var deleted = await _userService.DeleteUserByEmailAsync(email);
        if (!deleted)
        {
            _logger.LogWarning("DeleteUserByEmail: User with email '{Email}' not found.", email);
            return NotFound();
        }

        _logger.LogInformation("DeleteUserByEmail: User with email '{Email}' deleted.", email);
        return NoContent();
    }

    [HttpPut("{userId}/setactivate")]
    public async Task<IActionResult> SetActivateOwner(int userId)
    {
        var success = await _userService.SetActivateUserAsync(userId);
        if (!success)
        {
            _logger.LogWarning("SetActivateOwner: User with ID {UserId} not found or already inactive.", userId);
            return NotFound("User not found or already inactive.");
        }

        _logger.LogInformation("SetActivateOwner: User with ID {UserId} activation status toggled.", userId);
        return Ok("User activation status updated.");
    }
}