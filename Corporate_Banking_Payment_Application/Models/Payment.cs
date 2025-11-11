using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [ForeignKey("Beneficiary")]
        public int BeneficiaryId { get; set; }



        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
    );

        public DateTime? ProcessedDate { get; set; }

        [Required]
        public Status PaymentStatus { get; set; } = Status.PENDING;

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? RejectReason { get; set; }

        public Client? Client { get; set; }
        public Beneficiary? Beneficiary { get; set; }
    }
}
