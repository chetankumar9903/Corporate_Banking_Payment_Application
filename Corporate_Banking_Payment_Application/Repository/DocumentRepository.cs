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

        //public async Task<IEnumerable<Document>> GetAllDocuments()
        //{
        //    // Include Customer navigation property for richer data fetching
        //    return await _context.Documents
        //        .Include(d => d.Customer)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        public async Task<PagedResult<Document>> GetAllDocuments(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {

            var query = _context.Documents
                .Include(d => d.Customer).ThenInclude(c => c.User)
                .AsNoTracking();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(d =>
                    d.DocumentName.ToLower().Contains(searchTerm) ||
                    d.DocumentType.ToLower().Contains(searchTerm) ||
                    (d.Customer != null && d.Customer.User != null && (
                        d.Customer.User.FirstName.ToLower().Contains(searchTerm) ||
                        d.Customer.User.LastName.ToLower().Contains(searchTerm)
                    ))
                );
            }


            var totalCount = await query.CountAsync();


            bool isDescending = sortOrder == SortOrder.DESC;

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                switch (sortColumn.ToLower())
                {
                    case "documentname":
                        query = isDescending ? query.OrderByDescending(d => d.DocumentName) : query.OrderBy(d => d.DocumentName);
                        break;
                    case "documenttype":
                        query = isDescending ? query.OrderByDescending(d => d.DocumentType) : query.OrderBy(d => d.DocumentType);
                        break;
                    case "filesize":
                        query = isDescending ? query.OrderByDescending(d => d.FileSize) : query.OrderBy(d => d.FileSize);
                        break;
                    case "lastname":
                        query = isDescending ? query.OrderByDescending(d => d.Customer.User.LastName) : query.OrderBy(d => d.Customer.User.LastName);
                        break;
                    case "uploaddate":
                    default:
                        query = isDescending ? query.OrderByDescending(d => d.UploadDate) : query.OrderBy(d => d.UploadDate);
                        break;
                }
            }
            else
            {

                query = query.OrderByDescending(d => d.UploadDate);
            }


            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Document>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Document?> GetDocumentById(int id)
        {

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

            return await _context.Documents
                .Where(d => d.CustomerId == customerId)
                .Include(d => d.Customer)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
