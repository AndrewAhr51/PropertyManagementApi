using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security;

namespace PropertyManagementAPI.Domain.Entities
{
    public class RolePermission
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
