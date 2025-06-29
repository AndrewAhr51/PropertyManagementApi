namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class ElectronicTransferPayment : Payment
    {
        public string BankAccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string TransactionId { get; set; }
    }

}
