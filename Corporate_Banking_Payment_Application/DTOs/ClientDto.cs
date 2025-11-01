using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization; // Required for string-based enum in Swagger/API

namespace Corporate_Banking_Payment_Application.DTOs
{

    [JsonConverter(typeof(JsonStringEnumConverter))] // Makes Swagger show "DEPOSIT" instead of 0
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

        // Related Data
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

        [Range(0, (double)decimal.MaxValue)] // Use (double)decimal.MaxValue for range
        public decimal InitialBalance { get; set; } = 0;
    }

    public class UpdateClientDto
    {
        // ClientId should be passed via URL, not in the body for an update
        // public int ClientId { get; set; } 

        [Required, MaxLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    // --- UPDATED DTO FOR BALANCE UPDATE ---
    public class UpdateClientBalanceDto
    {
        [Required]
        // The enum now controls the action
        public TransactionType TransactionType { get; set; }

        [Required]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be a positive value.")]
        // This is the amount to deposit or withdraw
        public decimal Amount { get; set; }
    }
}

