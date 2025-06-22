namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class SecurityDepositInvoice : Invoice
    {
        public bool IsRefundable { get; set; }
    }

}
