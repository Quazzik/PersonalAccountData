using PersonalAccountData.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PersonalAccountData.Core.Entities;
using PersonalAccountData.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System;

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
    }
}
