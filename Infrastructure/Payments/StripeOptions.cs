using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Infrastructure.Payments
{
   

public class StripeOptions
{
    [Required]
    public string SecretKey { get; set; }

    [Required]
    public string PublishableKey { get; set; }
}
}
