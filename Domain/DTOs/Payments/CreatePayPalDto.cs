﻿using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class CreatePayPalDto
    {
        public int InvoiceId { get; set; }
        public int? TenantId { get; set; }
        public int? OwnerId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "USD";
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string Note { get; set; } = "Payment via PayPal";
        public string? PerformedBy { get; set; } = "Web";
        public Dictionary<string, string> Metadata { get; set; } = new(); // optional, initialized to avoid nulls
    }

}
