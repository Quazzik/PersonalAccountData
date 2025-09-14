using PersonalAccountData.Core.Entities;
using PersonalAccountData.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccountData.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Account> GetAccountByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAccountAsync(Account account)
        {
            if (account.StartDate >= account.EndDate && account.EndDate.HasValue)
                throw new ArgumentException("Start date must be before end date");
            await _repository.AddAsync(account);
            await _repository.SaveChangesAcync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _repository.UpdateAccount(account);
            await _repository.SaveChangesAcync();
        }

        public async Task DeleteAccountAsync(int id)
        {
            var account = await _repository.GetByIdAsync(id);
            if (account != null)
            {
                _repository.DeleteAccount(account);
                await _repository.SaveChangesAcync();
            }
        }
    }
}
