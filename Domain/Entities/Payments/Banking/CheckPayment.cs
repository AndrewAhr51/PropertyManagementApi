namespace PropertyManagementAPI.Domain.Entities.Payments.Banking
{
    public class CheckPayment : Payment
    {
        public string CheckNumber { get; set; }
        public string CheckBankName { get; set; }
    }


}
