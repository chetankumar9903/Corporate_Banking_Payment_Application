using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<Employee>> GetAllEmployees()
        //{
        //    // Include Client navigation property for richer data fetching
        //    return await _context.Employees
        //        .Include(e => e.Client)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        public async Task<PagedResult<Employee>> GetAllEmployees(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            // Base query MUST include dependencies for searching and sorting
            var query = _context.Employees
                .Where(e => e.IsActive)
                .Include(e => e.Client)
                .AsNoTracking();

            // 1. SEARCHING
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(e =>
                    e.EmployeeCode.ToLower().Contains(searchTerm) ||
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    (e.LastName != null && e.LastName.ToLower().Contains(searchTerm)) ||
                    e.EmailId.ToLower().Contains(searchTerm) ||
                    (e.Client != null && e.Client.CompanyName.ToLower().Contains(searchTerm))
                );
            }

            // Get TOTAL COUNT *after* searching
            var totalCount = await query.CountAsync();

            // 2. SORTING
            bool isDescending = sortOrder == SortOrder.DESC;

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                switch (sortColumn.ToLower())
                {
                    case "employeecode":
                        query = isDescending ? query.OrderByDescending(e => e.EmployeeCode) : query.OrderBy(e => e.EmployeeCode);
                        break;
                    case "firstname":
                        query = isDescending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName);
                        break;
                    case "emailid":
                        query = isDescending ? query.OrderByDescending(e => e.EmailId) : query.OrderBy(e => e.EmailId);
                        break;
                    case "salary":
                        query = isDescending ? query.OrderByDescending(e => e.Salary) : query.OrderBy(e => e.Salary);
                        break;
                    case "joindate":
                        query = isDescending ? query.OrderByDescending(e => e.JoinDate) : query.OrderBy(e => e.JoinDate);
                        break;
                    case "companyname":
                        query = isDescending ? query.OrderByDescending(e => e.Client.CompanyName) : query.OrderBy(e => e.Client.CompanyName);
                        break;
                    case "lastname":
                    default:
                        query = isDescending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName);
                        break;
                }
            }
            else
            {
                // Default sort
                query = query.OrderBy(e => e.LastName);
            }

            // 3. PAGINATION
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Employee>
            {
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<Employee?> GetEmployeeById(int id)
        {
            // Include Client navigation property for richer data fetching
            return await _context.Employees
                .Include(e => e.Client)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<Employee> AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task UpdateEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployee(Employee employee)
        {
            //_context.Employees.Remove(employee);
            //await _context.SaveChangesAsync();

            employee.IsActive = false;  // Soft delete
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByClientId(int clientId)
        {
            return await _context.Employees
                .Where(e => e.ClientId == clientId)
                //.Where(e => e.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
