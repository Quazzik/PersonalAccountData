using PersonalAccountData.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PersonalAccountData.Core.Entities;
using PersonalAccountData.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<bool> AccountNumberExistsAsync(string accountNumber)
        {
            return await _context.Accounts
                .AnyAsync(a => a.AccountNumber == accountNumber);
        }
    }
}
