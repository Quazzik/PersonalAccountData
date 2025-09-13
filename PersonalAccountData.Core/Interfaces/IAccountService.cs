using PersonalAccountData.Core.Entities;
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
    }
}