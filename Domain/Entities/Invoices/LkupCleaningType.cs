using PropertyManagementAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;

public class LkupCleaningType
{
    [Key]
    public int CleaningTypeId { get; set; }

    [Required]
    public string CleaningTypeName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

}