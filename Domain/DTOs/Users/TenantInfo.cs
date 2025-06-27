namespace PropertyManagementAPI.Domain.DTOs.Users
{
    public class TenantInfo
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public decimal RentAmount { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
