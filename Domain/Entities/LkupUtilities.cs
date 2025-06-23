using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities
{
    public class LkupUtilities
    {
        [Key]
        public int UtilityId { get; set; }
        [Required]
        public string UtilityName { get; set; } = null!;
        [Required]
        public string? Description { get; set; } = string.Empty;
        [Required]
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; } = "Web";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
       
    }
}
