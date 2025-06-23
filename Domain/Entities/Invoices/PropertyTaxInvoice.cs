using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class PropertyTaxInvoice : Invoice
    {
        [Required]
        public DateTime TaxPeriodStart { get; set; }
        [Required]
        public DateTime TaxPeriodEnd { get; set; }

    }

}
