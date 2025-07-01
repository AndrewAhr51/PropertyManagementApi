using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services.TenantAnnouncements;
using PropertyManagementAPI.Domain.DTOs.TenantAnnouncements;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/announcements")]
    public class TenantAnnouncementsController : ControllerBase
    {
        private readonly ITenantAnnouncementService _service;
        private readonly ILogger<TenantAnnouncementsController> _logger;

        public TenantAnnouncementsController(
            ITenantAnnouncementService service,
            ILogger<TenantAnnouncementsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET: api/announcements
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var announcements = await _service.GetActiveTenantAnnouncementsAsync();
            return Ok(announcements);
        }

        // POST: api/announcements
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TenantAnnouncementCreateDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Create: TenantAnnouncementCreateDto was null.");
                return BadRequest("Invalid announcement data.");
            }

            var announcement = await _service.PostTenantAnnouncementAsync(dto.Title, dto.Message, dto.PostedBy);
            _logger.LogInformation("Created new announcement with ID: {Id}", announcement.AnnouncementId);
            return CreatedAtAction(nameof(GetAll), new { }, announcement);
        }

        // DELETE: api/announcements/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.SetActiveTenantAnnouncementAsync(id);
            if (!success)
            {
                _logger.LogWarning("Delete: Announcement ID {Id} not found or already inactive.", id);
                return NotFound();
            }

            _logger.LogInformation("Delete: Announcement ID {Id} was deactivated.", id);
            return NoContent();
        }
    }
}