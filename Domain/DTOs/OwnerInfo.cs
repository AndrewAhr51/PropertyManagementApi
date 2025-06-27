namespace PropertyManagementAPI.Domain.DTOs
{
    public class OwnerInfo
    {
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
