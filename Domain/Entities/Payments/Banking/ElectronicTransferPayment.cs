namespace PropertyManagementAPI.Domain.Entities.Payments.Banking
{
    public class ElectronicTransferPayment: Payment

    {
        public string? BankAccountNumber { get; set; }
        public string? RoutingNumber { get; set; }
    }
}
