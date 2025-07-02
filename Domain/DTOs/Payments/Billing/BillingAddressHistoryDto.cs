namespace PropertyManagementAPI.Domain.DTOs.Payments.Billing
{
    public class BillingAddressHistoryDto : BillingAddressDto
    {
        public DateTime ChangedOn { get; set; }
        public string AvsResult { get; set; }
        public bool IsVerified { get; set; }
    }

}
