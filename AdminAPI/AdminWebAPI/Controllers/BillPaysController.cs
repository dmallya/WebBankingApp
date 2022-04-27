#nullable disable
using Microsoft.AspNetCore.Mvc;
using AdminWebAPI.Data;
using AdminWebAPI.Models;
using AdminWebAPI.Repositories;

namespace AdminWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillPaysController : ControllerBase
    {
        private readonly BillPayRepository _billPayRepository;

        public BillPaysController(WebAPIContext context)
        {
            _billPayRepository = new(context);
        }

        // GET: api/BillPays
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillPay>>> GetBillPays()
        {
            return await _billPayRepository.Get();
        }

        // GET: api/BillPays/5
        [HttpGet("{id}")]
        public async Task<BillPay> SetBillPay(int id)
        {
            Console.WriteLine("Blocking or unblocking " + id);
            var billPay = await _billPayRepository.Get(id);
            return billPay;
        }
    }
}
