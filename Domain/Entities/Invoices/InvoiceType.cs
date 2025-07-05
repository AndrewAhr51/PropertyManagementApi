namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class InvoiceType
    {
        public int Id { get; set; }
        public string LineItemTypeName { get; set; } = string.Empty;
    }
}
