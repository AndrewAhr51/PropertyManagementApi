namespace PropertyManagementAPI.Domain.DTOs
{
    public class NoteDto
    {
        public int NoteId { get; set; }
        public int? TenantId { get; set; } // Nullable for tenant-specific notes
        public int? PropertyId { get; set; } // Nullable for property-specific notes
        public string NoteText { get; set; } // Note content
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
