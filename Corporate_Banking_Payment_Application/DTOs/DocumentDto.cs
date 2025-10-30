using System.ComponentModel.DataAnnotations;

namespace Corporate_Banking_Payment_Application.DTOs
{
    // 1. DTO for displaying document details (Read Operations)
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

    // 2. DTO for creating a new document record (Write Operation)
    // NOTE: This DTO only contains metadata (CustomerId and DocumentType).
    // The actual file content (IFormFile) will be passed as a separate parameter
    // in the controller action, alongside this DTO.
    public class CreateDocumentDto
    {
        [Required, MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }
    }

    // 3. DTO for updating an existing document record (Patch/Put Operations)
    // Allows updating only metadata and status, not the file itself or its references.
    public class UpdateDocumentDto
    {
        [MaxLength(100)]
        public string? DocumentName { get; set; }

        [MaxLength(50)]
        public string? DocumentType { get; set; }

        // Allows toggling the active status of the document
        public bool? IsActive { get; set; }
    }
}
