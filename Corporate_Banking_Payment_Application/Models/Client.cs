using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    [Index(nameof(AccountNumber), IsUnique = true)]
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("Bank")]
        public int BankId { get; set; }

        [Required, MaxLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required, Range(0, double.MaxValue)]
        public decimal Balance { get; set; } = 0;

        public bool IsActive { get; set; } = true;


        public Customer? Customer { get; set; }
        public Bank? Bank { get; set; }
        public ICollection<Employee>? Employees { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<SalaryDisbursement>? SalaryDisbursements { get; set; }
        public ICollection<BatchTransaction>? BatchTransactions { get; set; }
        public ICollection<Beneficiary>? Beneficiaries { get; set; }
        public ICollection<Report>? Reports { get; set; }

    }
}
