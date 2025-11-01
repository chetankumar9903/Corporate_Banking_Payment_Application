﻿using Corporate_Banking_Payment_Application.Data;
using Corporate_Banking_Payment_Application.Models;
using Corporate_Banking_Payment_Application.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Corporate_Banking_Payment_Application.Repository
{
    public class BankRepository : IBankRepository
    {
        private readonly AppDbContext _context;

        public BankRepository(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<Bank>> GetAllBank()
        //{
        //    return await _context.Banks
        //        .Include(b => b.User)
        //        .AsNoTracking()
        //        .ToListAsync();
        //}

        public async Task<PagedResult<Bank>> GetAllBank(string? searchTerm, string? sortColumn, SortOrder? sortOrder, int pageNumber, int pageSize)
        {
            // Base query MUST include dependencies for searching and sorting
            var query = _context.Banks
                .Include(b => b.User)
                .AsNoTracking();

            // 1. SEARCHING
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b =>
                    b.BankName.ToLower().Contains(searchTerm) ||
                    b.Branch.ToLower().Contains(searchTerm) ||
                    b.IFSCCode.ToLower().Contains(searchTerm) ||
                    (b.User != null && (
                        b.User.UserName.ToLower().Contains(searchTerm) ||
                        b.User.FirstName.ToLower().Contains(searchTerm) ||
                        b.User.LastName.ToLower().Contains(searchTerm)
                    ))
                );
            }

            // Get TOTAL COUNT *after* searching
            var totalCount = await query.CountAsync();

            // 2. SORTING
            bool isDescending = sortOrder == SortOrder.DESC;

            if (!string.IsNullOrWhiteSpace(sortColumn))
            {
                // Adhering to the rule of using lowercase property names
                switch (sortColumn.ToLower())
                {
                    case "branch":
                        query = isDescending ? query.OrderByDescending(b => b.Branch) : query.OrderBy(b => b.Branch);
                        break;
                    case "ifsccode":
                        query = isDescending ? query.OrderByDescending(b => b.IFSCCode) : query.OrderBy(b => b.IFSCCode);
                        break;
                    case "lastname": // User's last name
                        query = isDescending ? query.OrderByDescending(b => b.User.LastName) : query.OrderBy(b => b.User.LastName);
                        break;
                    case "bankname":
                    default:
                        query = isDescending ? query.OrderByDescending(b => b.BankName) : query.OrderBy(b => b.BankName);
                        break;
                }
            }
            else
            {
                // Default sort
                query = query.OrderBy(b => b.BankName);
            }

            // 3. PAGINATION
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Bank>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        public async Task<Bank?> GetBankById(int id)
        {
            return await _context.Banks
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BankId == id);
        }

        public async Task<Bank> AddBank(Bank bank)
        {
            _context.Banks.Add(bank);
            await _context.SaveChangesAsync();
            return bank;
        }

        public async Task UpdateBank(Bank bank)
        {
            _context.Banks.Update(bank);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBank(Bank bank)
        {
            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsBank(int id)
        {
            return await _context.Banks.AnyAsync(b => b.BankId == id);
        }
    }
}
