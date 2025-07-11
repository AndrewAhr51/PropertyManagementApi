using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services.OwnerAnnouncements;
using PropertyManagementAPI.Domain.DTOs.OwnerAnnouncements;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/owner-announcements")]
    [Tags("Owner-Announcements")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class OwnerAnnouncementsController : ControllerBase
    {
        private readonly IOwnerAnnouncementService _service;
        private readonly ILogger<OwnerAnnouncementsController> _logger;

        public OwnerAnnouncementsController(
            IOwnerAnnouncementService service,
            ILogger<OwnerAnnouncementsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/owner-announcements
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var announcements = await _service.GetAllOwnerAnnouncementsAsync();
            return Ok(announcements);
        }

        // POST: api/owner-announcements
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OwnerAnnouncementCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Create: Received null OwnerAnnouncement.");
                return BadRequest("Invalid announcement data.");
            }

            var created = await _service.PostOwnerAnnouncementAsync(dto.Title, dto.Message, dto.PostedBy);
            _logger.LogInformation("Create: Owner announcement created with ID {Id}.", created.AnnouncementId);
            return CreatedAtAction(nameof(GetAll), new { }, created);
        }

        // PATCH: api/owner-announcements/{id}/activate
        [HttpPatch("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var success = await _service.SetActiveOwnerAnnouncementAsync(id);
            if (!success)
            {
                _logger.LogWarning("Activate: Owner announcement ID {Id} not found.", id);
                return NotFound();
            }

            _logger.LogInformation("Activate: Owner announcement ID {Id} was toggled.", id);
            return NoContent();
        }
    }
}