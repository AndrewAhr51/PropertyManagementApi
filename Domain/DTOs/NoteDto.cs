namespace PropertyManagementAPI.Domain.DTOs
{
    public class NoteDto
    {
        public int NoteId { get; set; }
        public int CreatedBy { get; set; } // Foreign key reference to Users
        public int? TenantId { get; set; } // Nullable for tenant-specific notes
        public int? PropertyId { get; set; } // Nullable for property-specific notes
        public string NoteText { get; set; } // Note content
        public DateTime CreatedAt { get; set; }
    }
}
