#nullable disable
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;

namespace AdminWebAPI.Repositories
{
    public class LoginRepository : IRepository<Login>
    {
        WebAPIContext _context;

        public LoginRepository(WebAPIContext context)
        {
            _context = context;
        }

        public async Task<List<Login>> Get()
        {
            return await _context.Logins.ToListAsync();
        }

        public async Task<Login> Get(int id)
        {
            var stringID = id.ToString();

            var login = await _context.Logins.FindAsync(stringID);

            if (login.Auth == Authorization.Authorized)
            {
                login.Auth = Authorization.Unauthorized;
                await _context.SaveChangesAsync();
                Console.WriteLine("Changing Auth to unAuth");
            }
            else if (login.Auth == Authorization.Unauthorized)
            {
                login.Auth = Authorization.Authorized;
                await _context.SaveChangesAsync();
                Console.WriteLine("Changing unAuth to Auth");
            }

            return login;
        }

        public async void Post(Login login)
        {
        }

        public async void Update(Login login)
        {
        }

        public async void Put(int id, Login login)
        {
        }
    }
}
