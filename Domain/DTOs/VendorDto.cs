namespace PropertyManagementAPI.Domain.DTOs
{
    public class VendorDto
    {
        public int VendorId { get; set; }
        public string Name { get; set; }
        public int ServiceTypeId { get; set; }
        public string ContactEmail { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string AccountNumber { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true; 
    }
}

