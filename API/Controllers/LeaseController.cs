﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs;

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaseDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Create: Received null LeaseDto.");
                return BadRequest("Invalid lease data.");
            }

            var created = await _service.CreateAsync(dto);
            _logger.LogInformation("Create: Lease created with ID {Id}.", created.LeaseId);
            return CreatedAtAction(nameof(GetById), new { leaseId = created.LeaseId }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leases = await _service.GetAllAsync();
            _logger.LogInformation("GetAll: Retrieved {Count} leases.", leases.Count());
            return Ok(leases);
        }

        [HttpGet("{leaseId}")]
        public async Task<IActionResult> GetById(int leaseId)
        {
            var lease = await _service.GetByIdAsync(leaseId);
            if (lease == null)
            {
                _logger.LogWarning("GetById: Lease ID {Id} not found.", leaseId);
                return NotFound();
            }

            return Ok(lease);
        }

        [HttpPut("{leaseId}")]
        public async Task<IActionResult> Update(int leaseId, [FromBody] LeaseDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Update: Received null LeaseDto for ID {Id}.", leaseId);
                return BadRequest("Invalid lease data.");
            }

            var updated = await _service.UpdateAsync(leaseId, dto);
            if (!updated)
            {
                _logger.LogWarning("Update: Lease ID {Id} not found or inactive.", leaseId);
                return NotFound();
            }

            _logger.LogInformation("Update: Lease ID {Id} updated.", leaseId);
            return NoContent();
        }

        [HttpDelete("{leaseId}")]
        public async Task<IActionResult> Delete(int leaseId)
        {
            var deleted = await _service.DeleteAsync(leaseId);
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