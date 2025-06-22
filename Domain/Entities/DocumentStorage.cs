using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{
    public class DocumentStorage
    {
        [Key]
        public int DocumentStorageId { get; set; }

        [Required]
        public int DocumentId { get; set; } // Foreign key reference to Documents

        [ForeignKey("DocumentId")]
        public Document Document { get; set; } // Navigation property

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        public int DocumentTypeId { get; set; } // PDF, DOCX, JPG, etc.

        [Required]
        public byte[] FileData { get; set; } // Blob storage

        [Required]
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
