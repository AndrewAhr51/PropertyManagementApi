using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string CustomerName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [MaxLength(255)]
        public string Address1 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string State { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        [Required]
        public int Bedrooms { get; set; }

        [Required]
        public int Bathrooms { get; set; }

        [Required]
        public int SquareFeet { get; set; } // Fixed missing closing brace  

        [Column(TypeName = "decimal(10,2)")]
        public decimal PropertyTaxes { get; set; } // No changes needed here  

        [Column(TypeName = "decimal(10,2)")]
        public decimal Insurance { get; set; } // No changes needed here  

        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;

        // Navigation property (optional)  
        public ICollection<PropertyOwner> PropertyOwners { get; set; }
    }
}
