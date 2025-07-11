using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Vendors;
using PropertyManagementAPI.Domain.DTOs.Vendors;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/vendors")]
    [Tags("Vendor")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetAllVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }

        [HttpGet("{vendorId}")]
        public async Task<ActionResult<VendorDto>> GetVendorById(int vendorId)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(vendorId);
            if (vendor == null) return NotFound();
            return Ok(vendor);
        }

        [HttpPost]
        public async Task<ActionResult<VendorDto>> CreateVendor([FromBody] VendorDto vendorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdVendor = await _vendorService.CreateVendorAsync(vendorDto);
            return CreatedAtAction(nameof(GetVendorById), new { vendorId = createdVendor.VendorId }, createdVendor);
        }

        [HttpPut("{vendorId}")]
        public async Task<IActionResult> UpdateVendor(int vendorId, [FromBody] VendorDto vendorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _vendorService.UpdateVendorAsync(vendorId, vendorDto);
            if (!success) return NotFound();

            return NoContent();
        }

        [HttpDelete("{vendorId}")]
        public async Task<IActionResult> SetIsActive(int vendorId)
        {
            var success = await _vendorService.SetIsActiveAsync(vendorId);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
