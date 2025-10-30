using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string EmailId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Position { get; set; }
        public string? Department { get; set; }
        public decimal Salary { get; set; }
        public DateTime JoinDate { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; } = 0;
        public bool IsActive { get; set; }
    }

    // DTO for creating a new employee (write operation)
    public class CreateEmployeeDto
    {
        // ClientId is required to associate the employee with a client
        [Required]
        public int ClientId { get; set; }

        //[Required, MaxLength(20)]
        //public string EmployeeCode { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? LastName { get; set; }

        [Required, EmailAddress]
        public string EmailId { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Position { get; set; }

        [MaxLength(50)]
        public string? Department { get; set; }

        [Required]
        // You might want to add a Range attribute for salary validation
        public decimal Salary { get; set; }

        //[Required]
        //// The Model sets a default to DateTime.UtcNow, but for creation, it's safer to require a value
        //public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        //[Required]
        //// You might want a MaxLength for AccountNumber based on typical bank standards
        //public string AccountNumber { get; set; } = string.Empty;

        // Balance is optional for creation and can default to 0
        public decimal Balance { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    // DTO for updating an existing employee (patch/put operations)
    public class UpdateEmployeeDto
    {
        // Personal/Contact Information Updates
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? EmailId { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        // Employment Details Updates
        [MaxLength(50)]
        public string? Position { get; set; }

        [MaxLength(50)]
        public string? Department { get; set; }

        // Financial/Status Updates
        public decimal? Salary { get; set; }

        public decimal? Balance { get; set; }

        public bool? IsActive { get; set; }
    }
}
