using Corporate_Banking_Payment_Application.Models;
using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailId { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public UserRole UserRole { get; set; }
    }

    public class CreateUserDto
    {
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

        [Required]
        public UserRole UserRole { get; set; }
    }

    public class UpdateUserDto
    {
        [Required, MaxLength(50)]
        public string? FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string? LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string? EmailId { get; set; } = string.Empty;

        [Required, Phone]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string? Address { get; set; } = string.Empty;

        public bool? IsActive { get; set; } = true;

        [Required]
        public UserRole? UserRole { get; set; }
    }
}
