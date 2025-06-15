using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{
    public class Pricing
    {
        [Key]
        public int PriceId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal RentalAmount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DepositAmount { get; set; }

        [Required]
        public string LeaseTerm { get; set; }

        public bool UtilitiesIncluded { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PropertyId))]
        public Property Property { get; set; }
    }
}