﻿using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class InvoiceCreateDto
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int PropertyId { get; set; }
        public string? Notes { get; set; }
        public string InvoiceType { get; set; } = string.Empty;
    }
}
