using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Application.Services.Quickbooks
{
    public class QuickBooksOptions
    {
      
        public string ClientId { get; set; } = string.Empty;
              
        public string Secret { get; set; } = string.Empty;
              
        public string RedirectUri { get; set; } = string.Empty;
    }

}
