using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
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

        [Range(0, double.MaxValue)]
        public decimal InitialBalance { get; set; } = 0;
    }

    public class UpdateClientDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required, MaxLength(100)]
        public string CompanyName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
