using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{
    public class PropertyPhotos
    {
        [Key]
        public int PhotoId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        [MaxLength(500)]
        public string PhotoUrl { get; set; }

        [Required]
        [MaxLength(500)]
        public string Room { get; set; }

        [MaxLength(255)]
        public string Caption { get; set; }

        public DateTime? CreatedDate { get; set; }

        [ForeignKey(nameof(PropertyId))]
        public Properties Property { get; set; }
    }
}

