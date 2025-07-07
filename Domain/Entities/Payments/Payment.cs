using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Domain.Entities.Invoices;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidOn { get; set; }
        public string ReferenceNumber { get; set; } = default!;
        public int InvoiceId { get; set; }
        public int? TenantId { get; set; }
        public int? OwnerId { get; set; }
        public string PaymentType { get; set; } = default!;
        public string? CardType { get; set; }
        public string? Last4Digits { get; set; }
        public string? AuthorizationCode { get; set; }
        public string? CheckNumber { get; set; }
        public string? CheckBankName { get; set; }
        public string? TransactionId { get; set; }
        public Invoice Invoice { get; set; } = default!;
        public Tenant Tenant { get; set; } = default!;
        public Owner Owner { get; set; } = default!;
    }
}


