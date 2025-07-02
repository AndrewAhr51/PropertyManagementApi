using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Banking
{
    public class AddBankAccountDto
    {
        [Required]
        public string BankName { get; set; }

        [Required, StringLength(4)]
        public string Last4 { get; set; }

        [Required]
        public string StripeBankAccountId { get; set; }
    }

}
