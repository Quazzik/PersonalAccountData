using PersonalAccountData.Core.Entities;
using PersonalAccountData.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccountData.Infrastructure.Services
{
    public class AccountNumberGenerator : IAccountNumberGenerator
    {
        private readonly IAccountRepository _accountRepository;

        public AccountNumberGenerator(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<string> GenerateNextAccountNumberAsync()
        {
            long maxNumberValue = await _accountRepository.GetMaxAccountNumberValueAsync();

            if (maxNumberValue == 0)
            {
                return 1.ToString("D10");
            }

            long nextNumber = ++maxNumberValue;

            if (nextNumber > 9999999999L)
            {
                throw new InvalidOperationException("The maximum personal account number has been reached");
            }
            return nextNumber.ToString("D10");
        }
    }
}
