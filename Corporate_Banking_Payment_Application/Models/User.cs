using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public UserRole UserRole { get; set; }

        [Required, MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string EmailId { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;


        public Customer? Customer { get; set; }
        public Bank? Bank { get; set; }
        public ICollection<Report>? Reports { get; set; }
    }
}
