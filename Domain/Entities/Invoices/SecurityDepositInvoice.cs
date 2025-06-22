namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class SecurityDepositInvoice : Invoice
    {
        public decimal DepositAmount { get; set; }
        public bool IsRefundable { get; set; }
    }

}
