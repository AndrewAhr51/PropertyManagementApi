using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities;

public class Vendor
{
    [Key]
    public int VendorId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [Required]
    public int ServiceTypeId { get; set; }

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string ContactEmail { get; set; }

    [Required]
    [MaxLength(255)]
    public string ContactFirstName { get; set; }

    [Required]
    [MaxLength(255)]
    public string ContactLastName { get; set; }

    [MaxLength(20)]
    [Phone]
    public string PhoneNumber { get; set; }

    [MaxLength(255)]
    public string Address { get; set; }

    [MaxLength(255)]
    public string Address1 { get; set; }

    [MaxLength(100)]
    public string City { get; set; }

    [MaxLength(50)]
    public string State { get; set; }

    [MaxLength(20)]
    public string PostalCode { get; set; }

    [Required]
    [MaxLength(50)]
    public string AccountNumber { get; set; }

    public string Notes { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;


}