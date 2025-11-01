
using Corporate_Banking_Payment_Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    // DTO for requesting a new report generation (Input DTO)
    public class GenerateReportRequestDto
    {
        // ✅ FIX: Added ReportName
        [Required(ErrorMessage = "Report name is required.")]
        [MaxLength(100)]
        public string ReportName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Report type is required.")]
        public ReportType ReportType { get; set; }

        // Note: The UserId should be taken from the authenticated user context (like a JWT token),
        // not passed in the DTO, for security reasons. The controller will handle this.
        // public int UserId { get; set; } 

        [Required(ErrorMessage = "Output format (PDF or EXCEL) is required.")]
        public ReportOutputFormat OutputFormat { get; set; }

        // Optional filter properties
        public int? ClientId { get; set; }
        public Status? PaymentStatusFilter { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    // DTO for displaying a saved report record (Output DTO)
    public class ReportDto
    {
        public int ReportId { get; set; }
        public string ReportName { get; set; } = string.Empty;
        public ReportType ReportType { get; set; }
        public ReportOutputFormat OutputFormat { get; set; }
        public int GeneratedBy { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string FilePath { get; set; } = string.Empty;
    }

    // DTO used for mapping a new report record before saving to DB (Internal DTO)
    public class CreateReportDto
    {
        [Required]
        public string ReportName { get; set; } = string.Empty;

        [Required]
        public ReportType ReportType { get; set; }

        public ReportOutputFormat OutputFormat { get; set; }

        [Required]
        public int GeneratedBy { get; set; }

        [Required]
        public string FilePath { get; set; } = string.Empty;
    }


    public class TransactionReportDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public DateTime? Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class PaymentReportDto
    {
        public int PaymentId { get; set; }
        public int ClientId { get; set; }
        public int BeneficiaryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public Status PaymentStatus { get; set; }
        public string? Description { get; set; }
        public string? RejectReason { get; set; }
    }

    public class SalaryReportDto
    {
        public int SalaryDisbursementId { get; set; }
        public int ClientId { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public int? BatchId { get; set; }
    }
}
