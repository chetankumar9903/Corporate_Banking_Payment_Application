using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class BatchTransactionDto
    {

        public int BatchId { get; set; }
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }


        public IEnumerable<SalaryDisbursementDto>? SalaryDisbursements { get; set; }
    }


    public class CreateBatchTransactionDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public List<int> EmployeeIds { get; set; } = new();

        [Required, Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal TotalAmount { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
