using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Bank")]
        public int BankId { get; set; }

        [Required]
        public DateTime OnboardingDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
    );

        [Required]
        public Status VerificationStatus { get; set; } = Status.PENDING;

        public bool IsActive { get; set; } = true;


        public User? User { get; set; }
        public Bank? Bank { get; set; }
        public Client? Client { get; set; }

        public ICollection<Document>? Documents { get; set; }
    }
}
