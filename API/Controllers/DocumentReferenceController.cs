using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Documents;
using PropertyManagementAPI.Domain.DTOs.Documents;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/document-references")]
    [Tags("Document References")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class DocumentReferenceController : ControllerBase
    {
        private readonly IDocumentReferenceService _service;

        public DocumentReferenceController(IDocumentReferenceService service)
        {
            _service = service;
        }

        [HttpPost]
        [ActionName("Create Document Reference")]
        public async Task<ActionResult<DocumentReferenceDto>> Add(DocumentReferenceDto dto) =>
            Ok(await _service.AddReferenceAsync(dto));

        [HttpGet("document/{documentId:int}")]
        [ActionName("Get References By Document")]
        public async Task<ActionResult<IEnumerable<DocumentReferenceDto>>> ByDocument(int documentId) =>
            Ok(await _service.GetReferencesByDocumentIdAsync(documentId));

        [HttpGet("entity/{type}/{id:int}")]
        [ActionName("Get References By Entity")]
        public async Task<ActionResult<IEnumerable<DocumentReferenceDto>>> ByEntity(string type, int id) =>
            Ok(await _service.GetReferencesByEntityAsync(type, id));

        [HttpDelete("{referenceId:int}")]
        [ActionName("Remove Document Reference")]
        public async Task<IActionResult> Remove(int referenceId)
        {
            var success = await _service.RemoveReferenceAsync(referenceId);
            return success ? NoContent() : NotFound();
        }
    }
}
