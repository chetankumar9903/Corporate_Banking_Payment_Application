﻿using System.ComponentModel.DataAnnotations;
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
        public ReportType ReportType { get; set; } // Transaction, Payment, Salary, etc.

        // Stores the format (PDF or EXCEL) the report was generated in
        [Required]
        public ReportOutputFormat OutputFormat { get; set; }

        [ForeignKey("User")]
        public int GeneratedBy { get; set; }

        [Required]
        public DateTime GeneratedDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
        );

        // This path will hold the secure Cloudinary URL
        [Required, MaxLength(350)] // Increased MaxLength to accommodate full Cloudinary URLs
        public string FilePath { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation
        public User? User { get; set; }
    }
}
