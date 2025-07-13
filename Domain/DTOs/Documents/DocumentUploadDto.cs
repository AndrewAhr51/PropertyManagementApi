
namespace PropertyManagementAPI.Domain.DTOs.Documents
{
    public class DocumentUploadDto
    {
        public IFormFile File { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int UploadedByUserId { get; set; }
        public string? DocumentType { get; set; }
        public string Status { get; set; } = "Active";
        public string Description { get; set; } = string.Empty; 
    }
}
