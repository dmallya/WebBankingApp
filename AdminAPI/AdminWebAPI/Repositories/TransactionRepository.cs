#nullable disable
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;

namespace AdminWebAPI.Repositories
{
    public class TransactionRepository : IRepository<Transaction>
    {
        WebAPIContext _context;

        public TransactionRepository(WebAPIContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> Get()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction> Get(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            return transaction;
        }

        public async void Post(Transaction transaction)
        {
        }

        public async void Update(Transaction transaction)
        {
        }

        public async void Put(int id, Transaction transaction)
        {
        }
    }
}
