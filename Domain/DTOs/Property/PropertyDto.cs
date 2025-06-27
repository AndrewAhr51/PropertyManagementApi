using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.DTOs.Property
{
    public class PropertyDto
    {
        [Required]
        public int PropertyId { get; set; }

        [Required]
        [MaxLength(255)]
        public string PropertyName { get; set; }

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

        [Range(0, int.MaxValue)]
        public int Bedrooms { get; set; }

        [Range(0, int.MaxValue)]
        public int Bathrooms { get; set; }

        [Range(0, int.MaxValue)]
        public int SquareFeet { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PropertyTaxes { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Insurance { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsActive { get; set; } = true;

        // Optional: for linking to PropertyOwners
        public int OwnerId { get; set; }
        public decimal OwnershipPercentage { get; set; } = 100;
        
    }
}