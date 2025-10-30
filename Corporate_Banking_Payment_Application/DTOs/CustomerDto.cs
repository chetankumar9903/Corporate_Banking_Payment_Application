using Corporate_Banking_Payment_Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class CustomerDto
    {

        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int BankId { get; set; }
        public DateTime OnboardingDate { get; set; }
        public Status VerificationStatus { get; set; }
        public bool IsActive { get; set; }

        public string? UserName { get; set; }
        public string? BankName { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int BankId { get; set; }

        //public Status VerificationStatus { get; set; } = Status.PENDING;
    }

    public class UpdateCustomerDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public Status VerificationStatus { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
