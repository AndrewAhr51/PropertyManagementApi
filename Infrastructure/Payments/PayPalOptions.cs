using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Infrastructure.Payments
{
    public class PayPalOptions
    {
        [Required]
        public string ClientId { get; set; }

        [Required]
        public string Secret { get; set; }
    }

}
