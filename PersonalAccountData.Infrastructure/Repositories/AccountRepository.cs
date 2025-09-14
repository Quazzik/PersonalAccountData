using Microsoft.EntityFrameworkCore;
using PersonalAccountData.Core.Entities;
using PersonalAccountData.Core.Interfaces;
using PersonalAccountData.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PersonalAccountData.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Account> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.Residents)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(a => a.Residents)
                .ToListAsync();
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
        }

        public void UpdateAccount(Account account)
        {
            _context.Accounts.Update(account);
        }

        public void DeleteAccount(Account account)
        {
            _context.Accounts.Remove(account);
        }

        public async Task<bool> SaveChangesAcync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<long> GetMaxAccountNumberValueAsync()
        {
            var allAccountNumbers = await _context.Accounts
                .Select(a => a.AccountNumber)
                .ToListAsync();

            var validAccountNumbers = allAccountNumbers
                .Where(an => an != null && an.Length == 10 && an.All(char.IsDigit))
                .ToList();

            if (!allAccountNumbers.Any())
                return 0;

            long maxValue = 0;
            foreach (var number in allAccountNumbers)
            {
                if (long.TryParse(number, out long numericValue))
                    maxValue = Math.Max(maxValue, numericValue);
            }
            return maxValue;
        }

        public async Task<bool> AccountNumberExistsAsync(string accountNumber, int? excludeID = null)
        {
            if (accountNumber?.Length != 0 || !accountNumber.All(char.IsDigit))
                return false;

            var query = _context.Accounts.Where(a => a.AccountNumber == accountNumber);

            if (excludeID.HasValue)
                query = query.Where(a => a.Id != excludeID.Value);

            return await query.AnyAsync();
        }

        public async Task<List<Account>> GetFilteredAccountsAsync(
            string searchTerm,
            bool? hasResidents,
            DateTime? activeDate,
            string accountNumber = null,
            string address = null,
            string residentName = null)
        {
            var query = _context.Accounts
                .Include(a => a.Residents)
                .AsQueryable();

            if (!string.IsNullOrEmpty(accountNumber) && accountNumber.Length == 10 &&
                accountNumber.All(char.IsDigit))
            {
                query = query.Where(a => a.AccountNumber.Contains(accountNumber));
            }

            if (!string.IsNullOrEmpty(address))
                query = query.Where(a => a.Address.Contains(address));

            if (!string.IsNullOrEmpty(residentName))
            {
                var searchParts = residentName
                    .ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in searchParts)
                {
                    query = query.Where(a => a.Residents.Any(r =>
                    EF.Functions.Like(
                        (r.LastName + " " + r.FirstName + " " + r.MiddleName).ToLower(),
                        "%" + part + "%")));
                }
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchParts = searchTerm
                    .ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in searchParts)
                {
                    var searchTermLower = searchTerm.ToLower();
                    query = query.Where(a =>
                    a.AccountNumber.ToLower().Contains(searchTerm) ||
                    a.Address.ToLower().Contains(searchTerm) ||
                    a.Residents.Any(r =>
                        EF.Functions.Like(
                            (r.LastName + " " + r.FirstName + " " + r.MiddleName).ToLower(),
                            "%" + part + "%")));
                }
            }

            if (hasResidents.HasValue)
            {
                query = hasResidents.Value ?
                    query.Where(a => a.Residents.Any()) :
                    query.Where(a => !a.Residents.Any());
            }

            if (activeDate.HasValue)
            {
                query = query.Where(a => a.StartDate <= activeDate.Value &&
                    (!a.EndDate.HasValue || a.EndDate.Value >= activeDate.Value));
            }

            return await query.ToListAsync();
        }

        public async Task<List<Account>> GetSortedAccountsAsync(
            string sortBy,
            bool descending = false)
        {
            var query = _context.Accounts.Include(a => a.Residents).AsQueryable();

            query = (sortBy?.ToLower(), descending) switch
            {
                ("accountnumber", false) => query.OrderBy(a => a.AccountNumber),
                ("accountnumber", true) => query.OrderByDescending(a => a.AccountNumber),
                ("startdate", false) => query.OrderBy(a => a.StartDate),
                ("startdate", true) => query.OrderByDescending(a => a.StartDate),
                ("address", false) => query.OrderBy(a => a.Address),
                ("address", true) => query.OrderByDescending(a => a.Address),
                ("area", false) => query.OrderBy(a => a.Area),
                ("area", true) => query.OrderByDescending(a => a.Area),
                (_, false) => query.OrderBy(a => a.Id),
                (_, true) => query.OrderByDescending(a => a.Id)
            };

            return await query.ToListAsync();
        }
    }
}
