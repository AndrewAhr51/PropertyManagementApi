namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class CardPayment : Payment
    {
        public string OrderId { get; set; }
        public string Status { get; set; } 
        public string CardType { get; set; } // e.g., Visa, MasterCard
        public string Last4Digits { get; set; }
        public string AuthorizationCode { get; set; }
    }

}
