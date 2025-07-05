namespace PropertyManagementAPI.Domain.DTOs.Property
{
    public class PropertyInfoDto
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public int OwnerId { get; set; }
        public string TenantEmail { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
    }
}
