using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{
    public class ClientBeneficiary
    {
        [Key]
        public int ClientBeneficiaryId { get; set; }

        [ForeignKey("Client")]
        public int ClientId { get; set; }

        [ForeignKey("Beneficiary")]
        public int BeneficiaryId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public Client? Client { get; set; }
        public Beneficiary? Beneficiary { get; set; }
    }
}
