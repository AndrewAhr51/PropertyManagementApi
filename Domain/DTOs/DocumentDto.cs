using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class DocumentDto
    {
        public int DocumentId { get; set; }

        public int PropertyId { get; set; }

        public int TenantId { get; set; }

        public string FileName { get; set; }

        public string FileUrl { get; set; }

        public string Category { get; set; }

        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}