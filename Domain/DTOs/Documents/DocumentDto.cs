using System;
using System.Collections.Generic;

namespace PropertyManagementAPI.Domain.DTOs.Documents
{
    public class DocumentDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public string DocumentType { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }
        public int CreatedByUserId { get; set; }

        public bool IsEncrypted { get; set; }
        public string? Checksum { get; set; }
        public string? CorrelationId { get; set; }
        public string Status { get; set; } = "Active";

        // Optional: for upload APIs or binary streaming
        public byte[]? Content { get; set; }

        // Optional: linked entity references (tenant, property, owner, etc.)
        public List<DocumentReferenceDto> References { get; set; } = new();
    }
}