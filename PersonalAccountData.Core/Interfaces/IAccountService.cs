using PersonalAccountData.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalAccountData.Core.Interfaces
{
    public interface IAccountService
    {
        Task<List<Account>> GetAccountsAsync();
        Task<Account> GetAccountByIdAsync(int id);
        Task CreateAccountAsync(Account account);
        Task UpdateAccountAsync(Account account);
        Task DeleteAccountAsync(int id);

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