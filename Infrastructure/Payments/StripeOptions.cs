using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Infrastructure.Payments
{


    public class StripeOptions
    {
        public string ClientSecret { get; set; }

        [Required]
        public string SecretKey { get; set; }

        [Required]
        public string PublishableKey { get; set; }

        [Required]
        public string WebhookSecret { get; set; }
    }
}
