namespace PropertyManagementAPI.Domain.DTOs.Property
{
    public class PropertyAddressDto
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Address1 { get; set; }
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public decimal? Amount { get; set; }
    }

}
