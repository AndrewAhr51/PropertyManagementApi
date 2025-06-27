using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Notes
{

    public class Note
    {
        [Key]
        public int NoteId { get; set; }
        [Required]
        public int? TenantId { get; set; } // Nullable for tenant-specific notes
        [Required]
        public int? PropertyId { get; set; } // Nullable for property-specific notes
        [Required]
        public string NoteText { get; set; } // Note content
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy    
    }
}


