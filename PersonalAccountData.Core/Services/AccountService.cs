using Microsoft.Extensions.Options;
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
        private readonly IAccountNumberGenerator _numberGenerator;

        public AccountService(IAccountRepository repository, IAccountNumberGenerator numberGenerator)
        {
            _repository = repository;
            _numberGenerator = numberGenerator;
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
            ValidateAccountDates(account);

            if(string.IsNullOrEmpty(account.AccountNumber))
            {
                account.AccountNumber = await _numberGenerator.GenerateNextAccountNumberAsync();
            }
            else
            {
                if (!IsValidAccountNumber(account.AccountNumber))
                {
                    throw new ArgumentException("The account number must consist of 10 digits");
                }

                if (await _repository.AccountNumberExistsAsync(account.AccountNumber, null))
                {
                    throw new ArgumentException("Such a personal account number already exists");
                }
            }
            ValidateRequiredFields(account);

            await _repository.AddAsync(account);
            await _repository.SaveChangesAcync();

        }

        public async Task UpdateAccountAsync(Account account)
        {
            if (!IsValidAccountNumber(account.AccountNumber))
            {
                throw new ArgumentException("The account number must consist of 10 digits");
            }

            if (await _repository.AccountNumberExistsAsync(account.AccountNumber, null))
            {
                throw new ArgumentException("Such a personal account number already exists");
            }

            ValidateAccountDates(account);
            ValidateRequiredFields(account);

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

        private void ValidateRequiredFields(Account account)
        {
            if (string.IsNullOrEmpty(account.Address))
            {
                throw new ArgumentException("Address is required");
            }
            if(account.Area <= 0)
            {
                throw new ArgumentException("The area of the room must be a positive number");
            }
        }

        private bool IsValidAccountNumber(string accountNumber)
        {
            return accountNumber != null &&
                accountNumber.Length == 10 &&
                accountNumber.All(char.IsDigit);
        }

        private void ValidateAccountDates(Account account)
        {
            if (account.EndDate.HasValue && account.StartDate >= account.EndDate.Value)
            {
                throw new ArgumentException("The start date must be earlier than the end date");
            }
        }

        public async Task<List<Account>> GetFilteredAccountsAsync(
            string searchTerm,
            bool? hasResidents,
            DateTime? activeDate,
            string accountNumber = null,
            string address = null,
            string residentName = null)
        {
            return await _repository.GetFilteredAccountsAsync(
                searchTerm, hasResidents, activeDate, accountNumber, address, residentName);
        }

        public async Task<List<Account>> GetSortedAccountsAsync(
            string sortBy,
            bool descending = false)
        {
            return await _repository.GetSortedAccountsAsync(sortBy, descending);
        }
    }
}
