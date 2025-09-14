using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using PersonalAccountData.API.DTOs;
using PersonalAccountData.Core.Entities;
using PersonalAccountData.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalAccountData.API.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[Controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountsController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountDTO>>> GetAll()
        {
            var accounts = await _accountService.GetAccountsAsync();
            if (accounts == null) return NotFound();
            return Ok(_mapper.Map<List<AccountDTO>>(accounts));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountDTO>> GetByID(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null) return NotFound();
            return Ok(_mapper.Map<AccountDTO>(account));
        }

        [HttpPost]
        public async Task<ActionResult> Create (AccountDTO accountDTO)
        {
            var account = _mapper.Map<Account>(accountDTO);
            await _accountService.CreateAccountAsync(account);
            return Ok();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<AccountDTO>>> GetAccounts([FromQuery] AccountFilterDTO filter)
        {
            var accounts = await _accountService.GetFilteredAccountsAsync(
                filter.Search,
                filter.HasResidents,
                filter.ActiveDate,
                filter.AccountNumber,
                filter.Address,
                filter.ResidentName);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                accounts = await _accountService.GetSortedAccountsAsync(filter.SortBy, filter.SortDescending);
            }

            return Ok(_mapper.Map<List<AccountDTO>>(accounts));
        }
    }
}
