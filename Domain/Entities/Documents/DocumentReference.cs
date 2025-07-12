namespace PropertyManagementAPI.Domain.Entities.Documents
{
    public class DocumentReference
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }
        public string RelatedEntityType { get; set; } = string.Empty; 
        public int RelatedEntityId { get; set; }
        public string AccessRole { get; set; } = "Viewer";            
        public DateTime LinkedDate { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public Document Document { get; set; } = default!;

    }
}
