using Going.Plaid.Entity;

namespace PropertyManagementAPI.Domain.DTOs.Documents
{
    public class DocumentUploadDto
    {
        public string FileName { get; set; } = string.Empty;

        public int RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; } = string.Empty; // e.g., "Tenant", "Property", etc.

        public byte[] Content { get; set; } = Array.Empty<byte>();
        public int UploadedByUserId { get; set; }
        public string? DocumentType { get; set; }
        public string Status { get; set; } = "Active";

        public string Description { get; set; } = string.Empty; 
    }
}
