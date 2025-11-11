using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    [Index(nameof(EmployeeCode), IsUnique = true)]
    [Index(nameof(AccountNumber), IsUnique = true)]
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [Required, MaxLength(20)]
        public string EmployeeCode { get; set; } = string.Empty;

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

        [Required]
        public DateTime JoinDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
    );


        [Required]
        public string AccountNumber { get; set; } = string.Empty;

        public decimal Balance { get; set; } = 0;

        public bool IsActive { get; set; } = true;


        public Client? Client { get; set; }
        public ICollection<SalaryDisbursement>? SalaryDisbursements { get; set; }
    }
}
