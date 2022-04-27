using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebBankingApp.Models;
using WebBankingApp.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace WebBankingApp.Data.Tests
{
    [TestClass()]
    public class SeedDataTests
    {
        [TestMethod()]
        public async Task DepositMethodTest()
        {
            var options = new DbContextOptions<WebBankingAppContext>();
            
            var webBankingAppContext = new WebBankingAppContext(options);
            var customerController = new CustomerController(webBankingAppContext);

            //Customer customer = new()
            //{
            //    CustomerID = 420,
            //    Name = "AB",
            //    Address = "Mernda",
            //    Suburb = "Melbourne",
            //    State = "VIC",
            //    PostCode = "3000",
            //    Mobile = "123456789",
            //    TFN = "12345678901"
            //};

            //webBankingAppContext.Customers.Add(customer);

            //Account account = new()
            //{
            //    AccountNumber = 25,
            //    CustomerID = 420,
            //    Balance = 100
            //};

            //webBankingAppContext.Accounts.Add(account);

            await customerController.Deposit(25, 250, null);

            var fetchedAcc = await webBankingAppContext.Accounts.FindAsync(25);

            Assert.AreEqual(fetchedAcc.Balance, 250);
        }
    }
}