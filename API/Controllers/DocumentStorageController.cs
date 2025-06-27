using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Documents;
using PropertyManagementAPI.Domain.DTOs.Documents;

[ApiController]
[Route("api/[controller]")]
public class DocumentStorageController : ControllerBase
{
    private readonly IDocumentStorageService _documentStorageService;

    public DocumentStorageController(IDocumentStorageService documentStorageService)
    {
        _documentStorageService = documentStorageService;
    }

    [HttpGet("{documentStorageId}")]
    public async Task<ActionResult<DocumentStorageDto>> GetDocumentById(int documentStorageId)
    {
        var document = await _documentStorageService.GetDocumentByIdAsync(documentStorageId);
        if (document == null) return NotFound();

        return Ok(document);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentStorageDto>>> GetAllDocuments()
    {
        var documents = await _documentStorageService.GetAllDocumentsAsync();
        return Ok(documents);
    }

    [HttpPost]
    public async Task<ActionResult> UploadDocument([FromBody] DocumentStorageDto documentStorageDto)
    {
        if (documentStorageDto == null) return BadRequest();

        var success = await _documentStorageService.UploadDocumentAsync(documentStorageDto);
        if (!success) return StatusCode(500, "Error uploading document");

        return CreatedAtAction(nameof(GetDocumentById), new { documentStorageId = documentStorageDto.DocumentStorageId }, documentStorageDto);
    }

    [HttpDelete("{documentStorageId}")]
    public async Task<ActionResult> DeleteDocument(int documentStorageId)
    {
        var success = await _documentStorageService.DeleteDocumentAsync(documentStorageId);
        if (!success) return NotFound();

        return NoContent();
    }
}