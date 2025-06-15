using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class PaymentMethodsDto
    {
        public int PaymentMethodId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MethodName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}

