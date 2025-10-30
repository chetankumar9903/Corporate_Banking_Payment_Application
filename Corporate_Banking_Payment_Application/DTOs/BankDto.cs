using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class BankDto
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string IFSCCode { get; set; }
        public string ContactNumber { get; set; }
        public string EmailId { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }

    }

    public class CreateBankDto
    {
        [Required, MaxLength(100)]
        public string BankName { get; set; }

        [Required, MaxLength(100)]
        public string Branch { get; set; }

        [Required, MaxLength(15)]
        public string IFSCCode { get; set; }

        [Required, Phone]
        public string ContactNumber { get; set; }

        [Required, EmailAddress]
        public string EmailId { get; set; }

        [Required]
        public int UserId { get; set; }
    }

    public class UpdateBankDto
    {
        [Required, MaxLength(100)]
        public string? BankName { get; set; }

        [Required, MaxLength(100)]
        public string? Branch { get; set; }

        [Required, MaxLength(15)]
        public string? IFSCCode { get; set; }

        [Required, Phone]
        public string? ContactNumber { get; set; }

        [Required, EmailAddress]
        public string? EmailId { get; set; }

        public bool? IsActive { get; set; } = true;
    }
}
