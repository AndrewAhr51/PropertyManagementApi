using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Domain.Entities.Payments.Banking
{
    public class ACHAuthorization
    {
        public int ACHAuthorizationId { get; set; }
        public int TenantId { get; set; }
        public DateTime AuthorizedOn { get; set; }
        public string IPAddress { get; set; }
        public string Signature { get; set; } // Optional digital signature
        public bool IsRevoked { get; set; } = false;

        public Tenant Tenant { get; set; }
    }
}
