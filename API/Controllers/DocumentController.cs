using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Documents;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Common.Utilities;
using PropertyManagementAPI.Domain.DTOs.Documents;
using PropertyManagementAPI.Domain.DTOs.Other;

namespace PropertyManagementAPI.API.Controllers.Documents
{
    [ApiController]
    [Route("api/v1/documents")]
    [Tags("Documents")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;
        private readonly EncryptionDocHelper _encryptionDocHelper;

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger, EncryptionDocHelper _encryptionDocHelper)
        {
            _documentService = documentService;
            this._encryptionDocHelper = _encryptionDocHelper;
            _logger = logger;
        }

        [HttpPost("upload-document")]
        [ActionName("Upload Complete Document")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DocumentDto>> UploadDocument([FromForm] DocumentUploadDto uploadDto)
        {
            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error on '{Key}': {Error}", kvp.Key, error.ErrorMessage);
                    }
                }
                return BadRequest(ModelState);
            }

            if (uploadDto?.File == null || uploadDto.File.Length == 0)
            {
                _logger.LogWarning("Upload failed: No file provided.");
                return BadRequest("File is required for upload.");
            }

            var created = await _documentService.UploadCompleteDocumentAsync(uploadDto);

            if (created == null || created.Id <= 0)
            {
                _logger.LogWarning("Document creation failed.");
                return BadRequest("Unable to create document.");
            }

            var document = await _documentService.GetDocumentByIdAsync(created.Id);

            if (document == null)
            {
                _logger.LogWarning("Document not found after creation.");
                return NotFound("Document not found after creation.");
            }

            // 🧩 Return explicitly serialized 201 with JSON content
            return CreatedAtRoute("GetDocumentById", new { id = document.Id }, document);

        }

        [HttpGet("{documentId:int}")]
        [ActionName("Get Document By Id")]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DocumentDto>> GetById(int documentId)
        {
            _logger.LogInformation("Fetching document with ID: {DocumentId}", documentId);

            var document = await _documentService.GetDocumentByIdAsync(documentId);
            if (document is null)
            {
                _logger.LogWarning("Document not found: {DocumentId}", documentId);
                return NotFound();
            }

            _logger.LogInformation("Document retrieved: {DocumentId}", document.Id);
            return Ok(document);
        }

        // 📋 List All Documents
        [HttpGet("all")]
        [ActionName("List All Documents")]
        [ProducesResponseType(typeof(PagedResult<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<DocumentDto>>> GetAll([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var results = await _documentService.GetAllDocumentAsync();
            return Ok(results);
        }

        [HttpGet("get-paged-documents")]
        [ActionName("Documents with Paging")]
        [ProducesResponseType(typeof(PagedResult<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<DocumentDto>>> GetPagedDocuments([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 10)
        {
            var result = await _documentService.GetPagedDocumentsAsync(pageIndex, pageSize);
            return Ok(result);
        }

        // 🗑️ Delete Document
        [HttpDelete("delete/{documentId:int}")]
        [ActionName("Delete Document")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int documentId)
        {
            var success = await _documentService.DeleteDocumentAsync(documentId);
            if (!success) return NotFound();
            return NoContent();
        }

        // 🏠 Get Documents By Property
        [HttpGet("property/{propertyId:int}")]
        [ActionName("Get Documents By Property")]
        [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetByProperty(int propertyId)
        {
            var results = await _documentService.GetDocumentByPropertyIdAsync(propertyId);
            return Ok(results);
        }

        // 👤 Get Documents By Tenant
        [HttpGet("tenant/{tenantId:int}")]
        [ActionName("Get Documents By Tenant")]
        [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetByTenant(int tenantId)
        {
            var results = await _documentService.GetDocumentByTenantIdAsync(tenantId);
            return Ok(results);
        }

        // 💼 Get Documents By Owner
        [HttpGet("owner/{ownerId:int}")]
        [ActionName("Get Documents By Owner")]
        [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetByOwner(int ownerId)
        {
            var results = await _documentService.GetDocumentByOwnerIdAsync(ownerId);
            return Ok(results);
        }

        // 📝 Get Documents Created By User
        [HttpGet("createdby/{userId:int}")]
        [ActionName("Get Documents By Creator")]
        [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetByCreator(int userId)
        {
            var results = await _documentService.GetDocumentByCreatedByAsync(userId);
            return Ok(results);
        }

        // 🎛️ Get Documents By Access Role
        [HttpGet("role/{role}")]
        [ActionName("Get Documents By Access Role")]
        [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetByAccessRole(string role)
        {
            var results = await _documentService.GetDocumentByAccessRoleAsync(role);
            return Ok(results);
        }

        // 📎 Download Document Content
        [HttpGet("content/{documentId:int}/download")]
        [ActionName("Download Document Content")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadContent(int documentId)
        {
            var content = await _documentService.GetDocumentContentAsync(documentId);
            if (content == null) return NotFound();

            var doc = await _documentService.GetDocumentByIdAsync(documentId);
            var mimeType = doc?.MimeType ?? "application/octet-stream";
            return File(content, mimeType, doc?.Name);
        }

        [HttpGet("{id}", Name = "GetDocumentById")]
        public async Task<ActionResult<DocumentDto>> GetDocumentById(int id)
        {
            var document = await _documentService.GetDocumentByIdAsync(id);
            return Ok(document);
        }
    }
}