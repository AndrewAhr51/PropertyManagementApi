using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Property;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.User
{
    public class Tenant
    {
        [Key]
        public int TenantId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public bool PrimaryTenant { get; set; } = false;

        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? MoveInDate { get; set; }

        [Required]
        [Column("Balance", TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0.0m;
        
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
     
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<Lease> Leases { get; set; }
    }

}
