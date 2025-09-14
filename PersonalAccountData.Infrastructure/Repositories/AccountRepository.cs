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
            var accounts = _context.Accounts
                .Include(a => a.Residents)
                .AsEnumerable();

            if (!string.IsNullOrEmpty(accountNumber) && accountNumber.Length == 10 &&
                accountNumber.All(char.IsDigit))
            {
                accounts = accounts.Where(a => a.AccountNumber.ToLower().Contains(accountNumber));
            }

            if (!string.IsNullOrEmpty(address))
                accounts = accounts.Where(a => a.Address.ToLower().Contains(address.ToLower()));

            if (hasResidents.HasValue)
            {
                accounts = hasResidents.Value ?
                    accounts.Where(a => a.Residents.Any()) :
                    accounts.Where(a => !a.Residents.Any());
            }

            if (activeDate.HasValue)
            {
                accounts = accounts.Where(a => a.StartDate <= activeDate.Value &&
                    (!a.EndDate.HasValue || a.EndDate.Value >= activeDate.Value));
            }

            if (!string.IsNullOrEmpty(residentName))
            {
                var searchParts = residentName
                    .ToLower()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);


                foreach (var part in searchParts)
                {
                    accounts = accounts.Where(a => a.Residents.Any(r =>
                        r.LastName.ToLower().Contains(part) ||
                        r.FirstName.ToLower().Contains(part) ||
                        r.MiddleName.ToLower().Contains(part)
                        ));
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
                    accounts = accounts.Where(a =>
                        a.AccountNumber.ToLower().Contains(searchTermLower) ||
                        a.Address.ToLower().Contains(searchTermLower) ||
                        a.Residents.Any(r =>
                            r.LastName.ToLower().Contains(part) ||
                            r.FirstName.ToLower().Contains(part) ||
                            r.MiddleName.ToLower().Contains(part)));
                }
            }

            return accounts.ToList();
        }

        public async Task<List<Account>> GetSortedAccountsAsync(
            string sortBy,
            bool descending = false,
            List<Account> accounts = null)
        {
            var accountsQuer = accounts ?? await _context.Accounts.Include(a => a.Residents).ToListAsync();

            accountsQuer = (sortBy?.ToLower(), descending) switch
            {
                ("accountnumber", false) => accountsQuer.OrderBy(a => a.AccountNumber).ToList(),
                ("accountnumber", true) => accountsQuer.OrderByDescending(a => a.AccountNumber).ToList(),
                ("startdate", false) => accountsQuer.OrderBy(a => a.StartDate).ToList(),
                ("startdate", true) => accountsQuer.OrderByDescending(a => a.StartDate).ToList(),
                ("address", false) => accountsQuer.OrderBy(a => a.Address).ToList(),
                ("address", true) => accountsQuer.OrderByDescending(a => a.Address).ToList(),
                ("area", false) => accountsQuer.OrderBy(a => a.Area).ToList(),
                ("area", true) => accountsQuer.OrderByDescending(a => a.Area).ToList(),
                (_, false) => accountsQuer.OrderBy(a => a.Id).ToList(),
                (_, true) => accountsQuer.OrderByDescending(a => a.Id).ToList()
            };

            return accounts;
        }
    }
}
