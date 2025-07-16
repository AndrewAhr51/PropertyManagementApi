using PropertyManagementAPI.Domain.DTOs.Invoices.Mappers;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Domain.DTOs.Invoices
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }

        public string InvoiceType { get; set; } // e.g. Rent, Maintenance, etc.
        public int PropertyId { get; set; }
        public int TenantId { get; set; }
        public string? TenantName { get; set; }
        public string Email { get; set; } = default!;        

        public int LineItemId { get; set; } 

        public string? PropertyName { get; set; }      
        public string? OwnerName { get; set; }     
        
        public int? OwnerId { get; set; }

        public string ReferenceNumber { get; set; } = default!;
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }        
        public decimal BalanceDue => Amount - AmountPaid;

        public DateTime DueDate { get; set; }
        public decimal LastMonthDue { get; set; } 
        public decimal LastMonthPaid { get; set; } 
        public int RentMonth { get; set; } // 1-12 for Jan-Dec
        public int RentYear { get; set; }  // e.g. 2023
        public bool IsPaid { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public string CreatedBy { get; set; } = "Web";
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public List<InvoiceLineItemDto> LineItems { get; internal set; }
    }
}
