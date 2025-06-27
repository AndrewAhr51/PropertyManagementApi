using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Domain.Entities.Properties
{
    [PrimaryKey(nameof(PropertyId), nameof(OwnerId))]
    public class PropertyOwner
    {
        public int PropertyId { get; set; }
        public int OwnerId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal OwnershipPercentage { get; set; } = 100;

        public Propertys Property { get; set; }
        public Owner Owner { get; set; }
    }
}