namespace PropertyManagementAPI.Domain.Entities.Payments.Billing
{
    public class BillingAddressHistory
    {
        public int BillingAddressHistoryId { get; set; }
        public int BillingAddressId { get; set; }
        public string StreetLine1 { get; set; }
        public string StreetLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string AvsResult { get; set; }
        public bool IsVerified { get; set; }
        public DateTime ChangedOn { get; set; }
    }
}
