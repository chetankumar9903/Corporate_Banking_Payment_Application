using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }


        private IQueryable<Report> GetBaseQuery()
        {

            return _context.Reports.Include(r => r.User)
                                   .Include(r => r.Client);
        }


        public async Task<Report> AddReport(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }


        public async Task<Report?> GetReportById(int id)
        {
            return await _context.Reports
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ReportId == id);
        }



        public async Task<PagedResult<Report>> GetReportsByUserId(int userId, string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            var query = GetBaseQuery()
                .AsNoTracking()
                .Where(r => r.GeneratedBy == userId);


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(r =>
                    r.ReportName.ToLower().Contains(searchTerm) ||
                    r.ReportType.ToString().ToLower().Contains(searchTerm) ||
                    r.OutputFormat.ToString().ToLower().Contains(searchTerm) ||
                    (r.ClientId.HasValue && r.ClientId.Value.ToString().Contains(searchTerm)) ||
                    (r.Client != null && r.Client.CompanyName.ToLower().Contains(searchTerm))
                );
            }


            var totalCount = await query.CountAsync();


            bool isDescending = sortOrder == SortOrder.DESC;

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                switch (sortColumn.ToLower())
                {
                    case "reportname":
                        query = isDescending ? query.OrderByDescending(r => r.ReportName) : query.OrderBy(r => r.ReportName);
                        break;
                    case "reporttype":
                        query = isDescending ? query.OrderByDescending(r => r.ReportType) : query.OrderBy(r => r.ReportType);
                        break;
                    case "outputformat":
                        query = isDescending ? query.OrderByDescending(r => r.OutputFormat) : query.OrderBy(r => r.OutputFormat);
                        break;
                    case "generateddate":
                        query = isDescending ? query.OrderByDescending(r => r.GeneratedDate) : query.OrderBy(r => r.GeneratedDate);
                        break;
                    default:
                        query = isDescending ? query.OrderByDescending(r => r.GeneratedDate) : query.OrderBy(r => r.GeneratedDate);
                        break;
                }
            }
            else
            {

                query = query.OrderByDescending(r => r.GeneratedDate);
            }


            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Report>
            {
                Items = items,
                TotalCount = totalCount
            };
        }
    }
}
