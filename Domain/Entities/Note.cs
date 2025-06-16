using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{

    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        public int CreatedBy { get; set; } // Foreign key reference to Users

        [ForeignKey("CreatedBy")]
        public User User { get; set; } // Navigation property

        public int? TenantId { get; set; } // Nullable for tenant-specific notes

        public int? PropertyId { get; set; } // Nullable for property-specific notes

        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string NoteText { get; set; } // Note content

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}


