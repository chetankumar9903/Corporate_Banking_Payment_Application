using Corporate_Banking_Payment_Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class NestedClientDto
    {
        public string CompanyName { get; set; }
    }

    public class NestedBeneficiaryDto
    {
        public string BeneficiaryName { get; set; }
        public string AccountNumber { get; set; }
    }
    public class PaymentDto
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

        public NestedClientDto Client { get; set; }
        public NestedBeneficiaryDto Beneficiary { get; set; }
    }


    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Client ID is required.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Beneficiary ID is required.")]
        public int BeneficiaryId { get; set; }

        [Required(ErrorMessage = "Amount is required."), Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }


        [MaxLength(200)]
        public string? Description { get; set; }
    }


    public class UpdatePaymentDto
    {

        [Required(ErrorMessage = "Payment Status is required for status updates.")]
        public Status? PaymentStatus { get; set; }

        [MaxLength(200)]
        public string? RejectReason { get; set; }
    }
}
