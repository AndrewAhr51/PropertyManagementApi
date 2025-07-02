using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Infrastructure.Payments
{
    public class PlaidOptions
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string Secret { get; set; }

        [Required]
        [RegularExpression("sandbox|development|production", ErrorMessage = "Environment must be sandbox, development, or production")]
        public string Environment { get; set; }
    }

}
