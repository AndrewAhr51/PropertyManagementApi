using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class lkupCleaningType
    {
        [Key]
        public int CleaningTypeId { get; set; }

        [Required]
        public string CleaningTypeName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

    }
}