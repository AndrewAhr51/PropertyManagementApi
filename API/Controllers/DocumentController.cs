using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Domain.DTOs.Documents;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _service;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService service, ILogger<DocumentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DocumentDto dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("Create: Received null DocumentDto.");
                return BadRequest("Invalid document data.");
            }

            var created = await _service.CreateAsync(dto);
            _logger.LogInformation("Create: Document created with ID {Id}.", created.DocumentId);
            return CreatedAtAction(nameof(GetById), new { documentId = created.DocumentId }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _service.GetAllAsync();
            _logger.LogInformation("GetAll: Retrieved {Count} documents.", documents.Count());
            return Ok(documents);
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetById(int documentId)
        {
            var doc = await _service.GetByIdAsync(documentId);
            if (doc == null)
            {
                _logger.LogWarning("GetById: Document ID {Id} not found.", documentId);
                return NotFound();
            }

            return Ok(doc);
        }

        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetByPropertyId(int propertyId)
        {
            var docs = await _service.GetByPropertyIdAsync(propertyId);
            if (!docs.Any())
            {
                _logger.LogWarning("GetByPropertyId: No documents found for property ID {Id}.", propertyId);
                return NotFound();
            }

            return Ok(docs);
        }

        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetByTenantId(int tenantId)
        {
            var docs = await _service.GetByTenantIdAsync(tenantId);
            if (!docs.Any())
            {
                _logger.LogWarning("GetByTenantId: No documents found for tenant ID {Id}.", tenantId);
                return NotFound();
            }

            return Ok(docs);
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> Delete(int documentId)
        {
            var deleted = await _service.DeleteAsync(documentId);
            if (!deleted)
            {
                _logger.LogWarning("Delete: Document ID {Id} not found.", documentId);
                return NotFound();
            }

            _logger.LogInformation("Delete: Document ID {Id} deleted.", documentId);
            return NoContent();
        }
    }
}