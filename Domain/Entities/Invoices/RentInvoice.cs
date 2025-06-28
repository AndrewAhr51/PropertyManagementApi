using PropertyManagementAPI.Domain.Entities.Properties;
using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class RentInvoice : Invoice
    {
        public int RentMonth { get; set; }
        public int RentYear { get; set; }

        // Optional override or extension for clarity/context
        public int PropertyId { get; set; }
        public Propertys Property { get; set; } // Navigation property

        public Tenant Tenant { get; set; } // Navigation property
    }

}
