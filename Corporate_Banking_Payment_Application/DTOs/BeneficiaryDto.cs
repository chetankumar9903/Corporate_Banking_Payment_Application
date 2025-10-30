using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class BeneficiaryDto
    {
        public int BeneficiaryId { get; set; }
        public string BeneficiaryName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string? Branch { get; set; }
        public string? IfscCode { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public int ClientId { get; set; }
    }

    // For creating a new beneficiary
    public class CreateBeneficiaryDto
    {
        [Required, MaxLength(100)]
        public string BeneficiaryName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string BankName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Branch { get; set; }

        [MaxLength(50)]
        public string? IfscCode { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }

        [Phone, MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [Required]
        public int ClientId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    // For updating existing beneficiary
    public class UpdateBeneficiaryDto
    {
        [MaxLength(100)]
        public string? BeneficiaryName { get; set; }

        [MaxLength(50)]
        public string? BankName { get; set; }

        [MaxLength(50)]
        public string? Branch { get; set; }

        [MaxLength(50)]
        public string? IfscCode { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }

        [Phone, MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public bool? IsActive { get; set; }
    }
}
