#nullable disable
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;

namespace AdminWebAPI.Repositories
{
    public class BillPayRepository : IRepository<BillPay>
    {
        WebAPIContext _context;

        public BillPayRepository(WebAPIContext context)
        {
            _context = context;
        }


        public async Task<List<BillPay>> Get()
        {

            return await _context.BillPays.ToListAsync();
        }

        public async Task<BillPay> Get(int id)
        {
            Console.WriteLine("Blocking or unblocking " + id);
            var billPay = await _context.BillPays.FindAsync(id);

            if (billPay.Period == PeriodType.Blocked)
            {
                billPay.Period = PeriodType.OneOff;
                await _context.SaveChangesAsync();
            }
            else
            {
                billPay.Period = PeriodType.Blocked;
                await _context.SaveChangesAsync();
            }

            return billPay;
        }

        public async void Post(BillPay billPay)
        {
            return;
        }

        public async void Update(BillPay billPay)
        {
            return;
        }

        public async void Put(int id, BillPay billPay)
        {
            return;
        }
    }
}
