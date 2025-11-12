using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [Required, MaxLength(100)]
        public string ReportName { get; set; } = string.Empty;

        [Required]
        public ReportType ReportType { get; set; }
        [Required]
        public ReportOutputFormat OutputFormat { get; set; }

        [ForeignKey("User")]
        public int GeneratedBy { get; set; }

        [ForeignKey("Client")]
        public int? ClientId { get; set; }

        [Required]
        public DateTime GeneratedDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        );

        [Required, MaxLength(350)]
        public string FilePath { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;


        public User? User { get; set; }
        public Client? Client { get; set; }
    }
}
