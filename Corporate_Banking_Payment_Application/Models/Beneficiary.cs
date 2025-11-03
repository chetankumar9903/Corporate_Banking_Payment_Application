using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    [Index(nameof(AccountNumber), IsUnique = true)]
    public class Beneficiary
    {
        [Key]
        public int BeneficiaryId { get; set; }

        [Required, MaxLength(100)]
        public string BeneficiaryName { get; set; } = string.Empty;

        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string BankName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Branch { get; set; }

        [StringLength(50)]
        public string? IfscCode { get; set; }


        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;


        // Foreign Key
        [Required]
        public int ClientId { get; set; }

        // Navigation Properties
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public ICollection<Payment>? Payments { get; set; }
    }
}
