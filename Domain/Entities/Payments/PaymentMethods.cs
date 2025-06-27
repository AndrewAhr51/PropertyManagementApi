using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class PaymentMethods
    {
        [Key]
        public int PaymentMethodId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MethodName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}