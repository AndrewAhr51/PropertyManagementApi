using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        public int PropertyId { get; set; }

        public int TenantId { get; set; }

        [MaxLength(255)]
        public string FileName { get; set; }

        [MaxLength(500)]
        public string FileUrl { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}