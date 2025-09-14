using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PersonalAccountData.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccountData.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> GetByIdAsync(int id);
        Task<List<Account>> GetAllAsync();
        Task AddAsync(Account account);
        void UpdateAccount (Account account);
        void DeleteAccount(Account account);
        Task<bool> SaveChangesAcync();
        Task<bool> AccountNumberExistsAsync(string accountNumber, int? excludeID);
        Task<long> GetMaxAccountNumberValueAsync();

        Task<List<Account>> GetFilteredAccountsAsync(
            string searchTerm,
            bool? hasResidents,
            DateTime? activeDate,
            string accountNumber = null,
            string address = null,
            string residentName = null);

        Task<List<Account>> GetSortedAccountsAsync(
            string sortBy,
            bool descending = false);
    }
}
