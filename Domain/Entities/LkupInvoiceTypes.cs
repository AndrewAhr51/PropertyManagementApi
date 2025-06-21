using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities
{
    public class LkupInvoiceType
    {
        [Key]

        public int InvoiceTypeId { get; set; }
        [Required]
        public string InvoiceType { get; set; } = null!;
        [Required]
        public string? Description { get; set; }
        public string CreatedBy { get; set; }  = "Web";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
