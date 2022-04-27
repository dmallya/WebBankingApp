#nullable disable
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;

namespace AdminWebAPI.Repositories
{
    public class AccountRepository : IRepository<Account>
    {
        WebAPIContext _context;

        public AccountRepository(WebAPIContext context)
        {
            _context = context;
        }


        public async Task<List<Account>> Get()
        {

            return await _context.Accounts.ToListAsync();
        }

        public async Task<Account> Get(int id)
        {

            var account = await _context.Accounts.FindAsync(id);

            return account;
        }

        public async void Post(Account account)
        {
            return;
        }

        public async void Update(Account account)
        {
            return;
        }

        public async void Put(int id, Account account)
        {
            return;
        }
    }
}
