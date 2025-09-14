using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccountData.Core.Interfaces
{
    public interface IAccountNumberGenerator
    {
        Task<string> GenerateNextAccountNumberAsync();
    }
}
