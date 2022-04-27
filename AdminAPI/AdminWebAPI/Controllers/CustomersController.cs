#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;
using AdminWebAPI.Repositories;

namespace AdminWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerRepository _customerRepository;

        public CustomersController(WebAPIContext context)
        {
            _customerRepository = new(context);
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _customerRepository.Get();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<Customer> GetCustomer(int id)
        {
            var customer = await _customerRepository.Get(id);

            return customer;
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public void PutCustomer(int id, Customer customer)
        {
            _customerRepository.Put(id, customer);
        }
    }
}
