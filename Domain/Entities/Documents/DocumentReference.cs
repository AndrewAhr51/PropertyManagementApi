namespace PropertyManagementAPI.Domain.Entities.Documents
{
    public class DocumentReference
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; } = null!;

        public string RelatedEntityType { get; set; } = string.Empty; // e.g. "Tenant", "Owner", "Property"
        public int RelatedEntityId { get; set; }

        public string AccessRole { get; set; } = "Viewer";             // "Uploader", "Signer", etc.
        public DateTime LinkDate { get; set; } = DateTime.UtcNow;
        public int LinkedByUserId { get; set; }

        public string? Description { get; set; }
    }
}
