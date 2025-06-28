namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class BankAccountDto
    {
        public string BankName { get; set; }
        public string AccountNumberMasked { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountType { get; set; }
    }
}
