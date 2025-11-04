using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IDocumentService
    {
        // Retrieval Operations
        //Task<IEnumerable<DocumentDto>> GetAllDocuments();
        Task<PagedResult<DocumentDto>> GetAllDocuments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<DocumentDto?> GetDocumentById(int id);
        Task<IEnumerable<DocumentDto>> GetDocumentsByCustomerId(int customerId);

        // Creation / Upload Operation
        // This method receives the file stream (IFormFile) and the metadata (DTO)
        Task<DocumentDto> UploadDocument(IFormFile file, CreateDocumentDto dto);

        // Update Operation (Likely only for IsActive status or DocumentType)
        Task<DocumentDto?> UpdateDocument(int id, UpdateDocumentDto dto);

        // Deletion Operation
        // This handles deleting the record from the database AND the file from Cloudinary
        Task<bool> DeleteDocument(int id);

        //Task<string?> GetTemporaryViewUrl(int id);
        //Task<string?> GetSignedDownloadUrl(int id);
    }
}
