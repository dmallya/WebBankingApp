#nullable disable
using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Data;
using AdminWebAPI.Models;
using AdminWebAPI.Repositories;

namespace AdminWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountRepository _accountRepository;

        public AccountsController(WebAPIContext context)
        {
            _accountRepository = new(context);
        }

        // GET: api/Accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _accountRepository.Get();
        }

        // GET: api/Accounts/5
        [HttpGet("{id}")]
        public async Task<Account> GetAccount(int id)
        {
            var account = await _accountRepository.Get(id);

            return account;
        }
    }
}
