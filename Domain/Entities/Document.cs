using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        public int? PropertyId { get; set; }

        public int? TenantId { get; set; }

        [MaxLength(255)]
        public string FileName { get; set; }

        [MaxLength(500)]
        public string FileUrl { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}