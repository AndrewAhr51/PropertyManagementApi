namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class PaymentMetadata
    {
        public int PaymentMetadataId { get; set; }

        public int PaymentId { get; set; }
        public Payment Payment { get; set; }

        public string Key { get; set; }      // e.g. "CardType", "BankName"
        public string Value { get; set; }    // e.g. "Visa", "Chase"
    }
}
