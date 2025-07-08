namespace PropertyManagementAPI.Domain.DTOs.Payments.PayPal
{
    public class PayPalCaptureResponseDto
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
        public string CaptureId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime CaptureTime { get; set; }
        public string PayerEmail { get; set; }
        public string PayerName { get; set; }
    }
}
