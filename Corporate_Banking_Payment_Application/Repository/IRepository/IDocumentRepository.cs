using Corporate_Banking_Payment_Application.Models;
namespace Corporate_Banking_Payment_Application.Repository.IRepository
{
    public interface IDocumentRepository
    {
        // CRUD Operations
        Task<IEnumerable<Document>> GetAllDocuments();
        Task<Document?> GetDocumentById(int id);
        Task<Document> AddDocument(Document document);
        Task UpdateDocument(Document document);
        Task DeleteDocument(Document document);

        // Utility/Query Methods
        Task<bool> ExistsDocument(int id);

        // Custom query to find all documents belonging to a specific customer
        Task<IEnumerable<Document>> GetDocumentsByCustomerId(int customerId);
    }
}
