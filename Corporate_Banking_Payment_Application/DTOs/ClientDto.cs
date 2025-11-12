using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Corporate_Banking_Payment_Application.DTOs
{

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionType
    {
        DEPOSIT,
        WITHDRAW
    }

    public class ClientDto
    {
        public int ClientId { get; set; }
        public int CustomerId { get; set; }
        public int BankId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }


        public string? CustomerName { get; set; }
        public string? BankName { get; set; }
    }

    public class CreateClientDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int BankId { get; set; }

        [Required, MaxLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        [Range(0, (double)decimal.MaxValue)]
        public decimal InitialBalance { get; set; } = 0;
    }

    public class UpdateClientDto
    {


        [Required, MaxLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }


    public class UpdateClientBalanceDto
    {
        [Required]

        public TransactionType TransactionType { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be a positive value.")]

        public decimal Amount { get; set; }
    }
}

