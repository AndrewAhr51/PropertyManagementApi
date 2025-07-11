using System;

namespace PropertyManagementAPI.Domain.DTOs.Documents
{
    public class DocumentReferenceDto
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }

        public string RelatedEntityType { get; set; } = string.Empty; // "Tenant", "Property", etc.
        public int RelatedEntityId { get; set; }

        public string AccessRole { get; set; } = "Viewer";
        public DateTime LinkDate { get; set; }
        public int LinkedByUserId { get; set; }

        public string? Description { get; set; }
    }
}