﻿using PropertyManagementAPI.Domain.Entities.Payments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.User
{
    public class Owner
    {
        [Key]
        public int OwnerId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public bool PrimaryOwner { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address1 { get; set; }

        [MaxLength(255)]
        public string? Address2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(100)]
        public string State { get; set; }

        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        public bool IsActive { get; set; } = true;

        // ✅ Navigation property with explicit foreign key
        [ForeignKey(nameof(UserId))]
        public Users Users { get; set; }

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}