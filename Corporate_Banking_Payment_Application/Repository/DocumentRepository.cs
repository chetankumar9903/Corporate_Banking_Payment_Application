using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly AppDbContext _context;

        public DocumentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllDocuments()
        {
            // Include Customer navigation property for richer data fetching
            return await _context.Documents
                .Include(d => d.Customer)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentById(int id)
        {
            // Include Customer navigation property
            return await _context.Documents
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(d => d.DocumentId == id);
        }

        public async Task<Document> AddDocument(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task UpdateDocument(Document document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDocument(Document document)
        {
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsDocument(int id)
        {
            return await _context.Documents.AnyAsync(d => d.DocumentId == id);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByCustomerId(int customerId)
        {
            // Custom query to efficiently retrieve documents for a specific customer
            return await _context.Documents
                .Where(d => d.CustomerId == customerId)
                .Include(d => d.Customer)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
