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
    public class LoginsController : ControllerBase
    {
        private readonly LoginRepository _loginRepository;

        public LoginsController(WebAPIContext context)
        {
            _loginRepository = new(context);
        }

        // GET: api/Logins
        [HttpGet]
        public async Task<IEnumerable<Login>> GetLogins()
        {
            return await _loginRepository.Get();
        }

        // GET: api/Logins/5
        [HttpGet("{id}")]
        public async Task<Login> GetLogin(string id)
        {
            var intID = Convert.ToInt32(id);
            var login = await _loginRepository.Get(intID);

            return login;
        }
    }
}
