
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{
    [Table("Permissions")]
    public class Permissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Example: "CanEditProperties"

        [MaxLength(255)]
        public string Description { get; set; } // ✅ Brief explanation of what the permission allows

    }
}
