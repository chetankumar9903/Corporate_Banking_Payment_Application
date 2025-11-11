using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    public class SalaryDisbursement
    {
        [Key]
        public int SalaryDisbursementId { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
    );

        [MaxLength(200)]
        public string? Description { get; set; }

        [ForeignKey("BatchTransaction")]
        public int? BatchId { get; set; }


        public Client? Client { get; set; }
        public Employee? Employee { get; set; }
        public BatchTransaction? BatchTransaction { get; set; }
    }
}
