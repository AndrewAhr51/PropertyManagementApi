using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class MaintenanceRequestDto
    {
        public int RequestId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public DateTime RequestDate { get; set; }

        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        public string PriorityLevel { get; set; }

        public string Status { get; set; }

        public string AssignedTo { get; set; }

        public string ResolutionNotes { get; set; }

        public DateTime? ResolvedDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}