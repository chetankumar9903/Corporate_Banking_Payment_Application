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


    public class CreateEmployeeDto
    {

        [Required]
        public int ClientId { get; set; }



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

        public decimal Salary { get; set; }


        public decimal Balance { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }


    public class UpdateEmployeeDto
    {

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? EmailId { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [MaxLength(50)]
        public string? Position { get; set; }

        [MaxLength(50)]
        public string? Department { get; set; }


        public decimal? Salary { get; set; }

        public decimal? Balance { get; set; }

        public bool? IsActive { get; set; }
    }
}
