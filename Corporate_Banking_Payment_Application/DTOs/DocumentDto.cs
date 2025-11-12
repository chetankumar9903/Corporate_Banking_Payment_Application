using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{

    public class DocumentDto
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string CloudinaryPublicId { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public int CustomerId { get; set; }
        public bool IsActive { get; set; }
    }


    public class CreateDocumentDto
    {
        [Required, MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }
    }


    public class UpdateDocumentDto
    {
        [MaxLength(100)]
        public string? DocumentName { get; set; }

        [MaxLength(50)]
        public string? DocumentType { get; set; }

        public bool? IsActive { get; set; }
    }
}
