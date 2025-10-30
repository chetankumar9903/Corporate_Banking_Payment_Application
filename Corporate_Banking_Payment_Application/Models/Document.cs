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
        public string DocumentType { get; set; } = string.Empty; // e.g. ID Proof, Address Proof

        // CLOUDINARY FIELDS 
        [Required, MaxLength(150)]
        public string CloudinaryPublicId { get; set; } = string.Empty; // The identifier for management/deletion

        [Required, MaxLength(350)]
        public string FileUrl { get; set; } = string.Empty; // The full secured URL for retrieval
                                                            // ************************

        public long FileSize { get; set; } // in bytes

        [Required]
        // Setting the default time to India Standard Time (IST).
        public DateTime UploadDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
        DateTime.UtcNow,
        TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
   );

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public Customer? Customer { get; set; }
    }

    //public class Document
    //{
    //    [Key]
    //    public int DocumentId { get; set; }

    //    [Required, MaxLength(100)]
    //    public string DocumentName { get; set; } = string.Empty;

    //    [Required, MaxLength(50)]
    //    public string DocumentType { get; set; } = string.Empty; // e.g. ID Proof, Address Proof

    //    [Required, MaxLength(255)]
    //    public string FilePath { get; set; } = string.Empty;

    //    public long FileSize { get; set; } // in bytes

    //    [Required]
    //    //public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    //    public DateTime UploadDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
    //    DateTime.UtcNow,
    //    TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
    //);

    //    [ForeignKey("Customer")]
    //    public int CustomerId { get; set; }

    //    public bool IsActive { get; set; } = true;

    //    // Navigation
    //    public Customer? Customer { get; set; }
    //}
}
