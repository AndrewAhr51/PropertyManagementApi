using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Domain.DTOs;

[ApiController]
[Route("api/pricing")]
public class PricingController : ControllerBase
{
    private readonly IPricingService _service;
    private readonly ILogger<PricingController> _logger;

    public PricingController(IPricingService service, ILogger<PricingController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PricingDto dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("Create: Received null PricingDto.");
            return BadRequest("Invalid pricing data.");
        }

        var created = await _service.CreateAsync(dto);
        _logger.LogInformation("Create: Pricing created with ID {Id}.", created.PriceId);
        return CreatedAtAction(nameof(GetById), new { id = created.PriceId }, created);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _service.GetAllAsync();
        _logger.LogInformation("GetAll: Retrieved {Count} pricing records.", items.Count());
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null)
        {
            _logger.LogWarning("GetById: Pricing ID {Id} not found.", id);
            return NotFound();
        }

        return Ok(item);
    }

    [HttpGet("latest/{propertyId}")]
    public async Task<IActionResult> GetLatestForProperty(int propertyId)
    {
        var latest = await _service.GetLatestForPropertyAsync(propertyId);
        if (latest == null)
        {
            _logger.LogWarning("GetLatestForProperty: No pricing found for property ID {PropertyId}.", propertyId);
            return NotFound();
        }

        _logger.LogInformation("GetLatestForProperty: Retrieved latest pricing for property ID {PropertyId}.", propertyId);
        return Ok(latest);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PricingDto dto)
    {
        if (dto == null)
        {
            _logger.LogWarning("Update: Received null PricingDto for ID {Id}.", id);
            return BadRequest("Invalid pricing data.");
        }

        var updated = await _service.UpdateAsync(id, dto);
        if (!updated)
        {
            _logger.LogWarning("Update: Pricing ID {Id} not found.", id);
            return NotFound();
        }

        _logger.LogInformation("Update: Pricing ID {Id} updated.", id);
        return NoContent();
    }
}