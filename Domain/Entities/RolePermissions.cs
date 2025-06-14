using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace PropertyManagementAPI.Domain.Entities
{
    public class RolePermissions
    {
        [Column(Order = 1)]
        public int RolePermissionId { get; set; }

        [Column(Order = 2)]
        public int PermissionId { get; set; }

        // ✅ Corrected Navigation Properties
        [ForeignKey(nameof(RolePermissionId))]
        public virtual Roles Role { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public virtual Permissions Permission { get; set; }
    }
}
