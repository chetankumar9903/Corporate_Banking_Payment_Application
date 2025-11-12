using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    public class Bank
    {
        [Key]
        public int BankId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string BankName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Branch { get; set; } = string.Empty;

        [Required, MaxLength(15)]
        public string IFSCCode { get; set; } = string.Empty;

        [Required, Phone]
        public string ContactNumber { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string EmailId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;


        public User? User { get; set; }
        public ICollection<Customer>? Customers { get; set; }

        public ICollection<Client>? Clients { get; set; }
    }
}
