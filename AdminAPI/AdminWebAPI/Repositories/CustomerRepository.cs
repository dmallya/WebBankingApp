#nullable disable
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;

namespace AdminWebAPI.Repositories
{
    public class CustomerRepository : IRepository<Customer>
    {
        WebAPIContext _context;

        public CustomerRepository(WebAPIContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> Get()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> Get(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            return customer;
        }

        public async void Put(int id, Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return;
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }

        public async void Post(Customer customer)
        {
        }

        public async void Update(Customer customer)
        {
        }
    }
}
