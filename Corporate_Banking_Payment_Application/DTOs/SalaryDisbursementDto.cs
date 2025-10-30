using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class SalaryDisbursementDto
    {
        public int SalaryDisbursementId { get; set; }
        public int ClientId { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public int? BatchId { get; set; }

        // Optional: Employee/Client Details
        public string? EmployeeName { get; set; }
        public string? ClientCompanyName { get; set; }
    }

    // ✅ For Creating
    public class CreateSalaryDisbursementDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required, Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public int? BatchId { get; set; }
    }

    // ✅ For Updating
    public class UpdateSalaryDisbursementDto
    {
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal? Amount { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public int? BatchId { get; set; }
    }
}
