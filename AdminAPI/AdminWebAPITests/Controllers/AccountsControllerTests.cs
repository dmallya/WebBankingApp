using Xunit;
using Microsoft.EntityFrameworkCore;
using AdminWebAPI.Data;
using AdminWebAPI.Models;
using System.Threading.Tasks;

namespace AdminWebAPI.Controllers.Tests
{
    public class AllEndpointTests
    {
        [Theory]
        [InlineData(4100)]
        [InlineData(4101)]
        [InlineData(4200)]
        [InlineData(4300)]
        public async Task AccountsGetTest(int value)
        {

            var optionsBuilder = new DbContextOptions<WebAPIContext>();
            WebAPIContext context = new(optionsBuilder);

            AccountsController accountController = new(context);

            var account = await accountController.GetAccount(value);
            if (value == 4100 || value == 4101)
            {
                Assert.True(account.CustomerID == 2100);
                Assert.False(account.CustomerID == 1205);
                Assert.False(account.CustomerID == 0);
            }
            else if (value == 4200)
            {
                Assert.True(account.CustomerID == 2200);
                Assert.False(account.CustomerID == 4862);
                Assert.False(account.CustomerID == 0);
            }
            else if (value == 4300)
            {
                Assert.True(account.CustomerID == 2300);
                Assert.False(account.CustomerID == 9726);
                Assert.False(account.CustomerID == 0);
            }
        }

        [Theory]
        [InlineData(2100)]
        [InlineData(2200)]
        [InlineData(2300)]
        public async Task CustomerGetTest(int value)
        {

            var optionsBuilder = new DbContextOptions<WebAPIContext>();
            WebAPIContext context = new(optionsBuilder);

            CustomersController customersController = new(context);

            var customer = await customersController.GetCustomer(value);
            if (value == 2100)
            {
                Assert.True(customer.Name == "Matthew Bolger");
                Assert.False(customer.Name == "Rodney Cocker");
                Assert.False(customer.Name == "Shekhar Kalra");
            }
            else if (value == 2200)
            {
                Assert.False(customer.Name == "Matthew Bolger");
                Assert.True(customer.Name == "Rodney Cocker");
                Assert.False(customer.Name == "Shekhar Kalra");
            }
            else if (value == 2300)
            {
                Assert.False(customer.Name == "Matthew Bolger");
                Assert.False(customer.Name == "Rodney Cocker");
                Assert.True(customer.Name == "Shekhar Kalra");
            }
        }

        [Theory]
        [InlineData(2100)]
        [InlineData(2200)]
        [InlineData(2300)]
        public async Task CustomerPutTest(int value)
        {
            var optionsBuilder = new DbContextOptions<WebAPIContext>();
            WebAPIContext context = new(optionsBuilder);

            CustomersController customersController = new(context);
            var customer = await customersController.GetCustomer(value);
            customer.State = "VIC";
            customersController.PutCustomer(value, customer);
            var fetchedCustomer = await customersController.GetCustomer(value);
            Assert.True(fetchedCustomer.State == "VIC");
            Assert.False(fetchedCustomer.State == "NSW");
            Assert.False(fetchedCustomer.State == "QLD");
            Assert.False(fetchedCustomer.State == "");
        }

        [Theory]
        [InlineData(12345678)]
        [InlineData(38074569)]
        [InlineData(17963428)]
        public async Task LoginLockTest(int value)
        {
            var optionsBuilder = new DbContextOptions<WebAPIContext>();
            WebAPIContext context = new(optionsBuilder);

            LoginsController loginsController = new(context);
            
            var login = await loginsController.GetLogin(value.ToString());

            if (login.Auth == Authorization.Authorized) 
            {
                var finalLogin = await loginsController.GetLogin(value.ToString());
                Assert.True(finalLogin.Auth == Authorization.Unauthorized);
            }
            if (login.Auth == Authorization.Unauthorized)
            {
                var finalLogin = await loginsController.GetLogin(value.ToString());
                Assert.True(finalLogin.Auth == Authorization.Authorized);
            }
        }
    }   
}