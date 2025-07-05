namespace PropertyManagementAPI.Domain.DTOs.Quickbooks
{
    public class QuickBooksOptions
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
        public string RedirectUri { get; set; } = null!;
    }
}
