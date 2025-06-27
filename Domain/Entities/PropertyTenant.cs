
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{

    [PrimaryKey(nameof(PropertyId), nameof(TenantId))]
    public class PropertyTenant
    {
        public int PropertyId { get; set; }

        public int TenantId { get; set; }
    }
}
