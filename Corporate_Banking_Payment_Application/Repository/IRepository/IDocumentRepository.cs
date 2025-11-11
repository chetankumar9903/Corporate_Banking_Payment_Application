using Corporate_Banking_Payment_Application.Models;
namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IDocumentRepository
    {

        //Task<IEnumerable<Document>> GetAllDocuments();
        Task<PagedResult<Document>> GetAllDocuments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize);
        Task<Document?> GetDocumentById(int id);
        Task<Document> AddDocument(Document document);
        Task UpdateDocument(Document document);
        Task DeleteDocument(Document document);

        Task<bool> ExistsDocument(int id);


        Task<IEnumerable<Document>> GetDocumentsByCustomerId(int customerId);
    }
}
