using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corporate_Banking_Payment_Application.Models
{


    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required, MaxLength(100)]
        public string DocumentName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;


        [Required, MaxLength(150)]
        public string CloudinaryPublicId { get; set; } = string.Empty;

        [Required, MaxLength(350)]
        public string FileUrl { get; set; } = string.Empty;

        public long FileSize { get; set; }

        [Required]
        public DateTime UploadDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
   );

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public bool IsActive { get; set; } = true;


        public Customer? Customer { get; set; }
    }
}
