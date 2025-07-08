namespace PropertyManagementAPI.Domain.DTOs.Payments.PayPal
{
    public class PayPalOrderInitResponse
    {
        public string OrderId { get; set; }
        public string ApproveUrl { get; set; }
    }
}
