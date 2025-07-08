namespace PropertyManagementAPI.Domain.DTOs.Payments.PayPal
{
    public class PayPalOrderResult
    {
        public string OrderId { get; set; }
        public string ApprovalLink { get; set; }
    }

}
