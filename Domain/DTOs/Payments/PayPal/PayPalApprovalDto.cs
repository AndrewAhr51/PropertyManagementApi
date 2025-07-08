namespace PropertyManagementAPI.Domain.DTOs.Payments.PayPal
{
    public class PayPalApprovalDto
    {
        public string OrderId { get; set; }
        public string ApprovalLink { get; set; }
    }
}
