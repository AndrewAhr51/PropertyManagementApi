using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementAPI.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // ✅ Create a new user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");

            var createdUser = await _userService.CreateUserAsync(userDto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.UserId }, createdUser);
        }

        // ✅ Get a user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // ✅ Update user details
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            if (userDto == null)
                return BadRequest("Invalid user data.");

            var updated = await _userService.UpdateUserAsync(id, userDto);
            if (!updated) return NotFound();

            return NoContent(); // ✅ HTTP 204 - Update successful
        }

        // ✅ Delete a user
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted) return NotFound();

            return NoContent(); // ✅ HTTP 204 - Deletion successful
        }
    }
}