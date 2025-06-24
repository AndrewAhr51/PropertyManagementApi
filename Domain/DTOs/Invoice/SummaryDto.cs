namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class SummaryDto
    {
        public decimal TotalAmount { get; set; }
        public int Count { get; set; }
        public Dictionary<string, decimal> BreakdownByType { get; set; } = new();
        public Dictionary<string, decimal> MonthlyTotals { get; set; } = new();
    }
}
