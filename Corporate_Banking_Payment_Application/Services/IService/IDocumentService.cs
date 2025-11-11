using Corporate_Banking_Payment_Application.DTOs;
using Corporate_Banking_Payment_Application.Models;

namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IDocumentService
    {

        //Task<IEnumerable<DocumentDto>> GetAllDocuments();
        Task<PagedResult<DocumentDto>> GetAllDocuments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<DocumentDto?> GetDocumentById(int id);
        Task<IEnumerable<DocumentDto>> GetDocumentsByCustomerId(int customerId);
        Task<DocumentDto> UploadDocument(IFormFile file, CreateDocumentDto dto);
        Task<DocumentDto?> UpdateDocument(int id, UpdateDocumentDto dto);
        Task<bool> DeleteDocument(int id);
    }
}
