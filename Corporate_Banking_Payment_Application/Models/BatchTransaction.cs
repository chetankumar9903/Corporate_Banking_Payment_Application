using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    public class BatchTransaction
    {
        [Key]
        public int BatchId { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [Required]
        public DateTime Date { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
    );

        [Required]
        public int TotalTransactions { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        // Navigation
        public Client? Client { get; set; }
        public ICollection<SalaryDisbursement>? SalaryDisbursements { get; set; }
    }
}
