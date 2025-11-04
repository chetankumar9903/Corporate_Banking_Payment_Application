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

    // DTO for creating a new payment request (Write Operation)
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Client ID is required.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Beneficiary ID is required.")]
        public int BeneficiaryId { get; set; }

        [Required(ErrorMessage = "Amount is required."), Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        // RequestDate and Status are typically set by the service layer, 
        // but included here if the consumer needs to provide a description/reference.
        [MaxLength(200)]
        public string? Description { get; set; }
    }

    // DTO for updating the status of an existing payment, primarily used by internal services/approvers.
    public class UpdatePaymentDto
    {
        // PaymentStatus is the ONLY required field for an update now
        [Required(ErrorMessage = "Payment Status is required for status updates.")]
        public Status? PaymentStatus { get; set; }

        // RejectReason is only relevant if the status is REJECTED
        [MaxLength(200)]
        public string? RejectReason { get; set; }
    }
}
