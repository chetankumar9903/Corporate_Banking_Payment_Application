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

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            // Include Client navigation property for richer data fetching
            return await _context.Employees
                .Include(e => e.Client)
                .AsNoTracking()
                .ToListAsync();
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
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByClientId(int clientId)
        {
            return await _context.Employees
                .Where(e => e.ClientId == clientId)
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
