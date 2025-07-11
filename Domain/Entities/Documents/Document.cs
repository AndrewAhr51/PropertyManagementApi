using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities.Documents
{
    public class Document
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public string DocumentType { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public int CreatedByUserId { get; set; }

        public bool IsEncrypted { get; set; } = false;
        public string? Checksum { get; set; }
        public string? CorrelationId { get; set; }
        public string Status { get; set; } = "Active";

        public byte[] Content { get; set; } = Array.Empty<byte>();

        public ICollection<DocumentReference> References { get; set; } = new List<DocumentReference>();
    }
}