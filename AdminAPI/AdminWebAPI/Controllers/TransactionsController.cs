#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;
using AdminWebAPI.Repositories;

namespace AdminWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly AccountRepository _accountRepository;

        public TransactionsController(WebAPIContext context)
        {
            _transactionRepository = new(context);
            _accountRepository = new(context);
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _transactionRepository.Get();
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<List<Transaction>> GetTransaction(int id)
        {
            var account = await _accountRepository.Get(id);

            return account.Transactions;
        }
    }
}
