using Microsoft.AspNetCore.Mvc;
using WebBankingApp.Data;
using WebBankingApp.Models;
using Newtonsoft.Json;
using WebBankingApp.Filters;
using SimpleHashing;
using System.Text;

namespace WebBankingApp.Controllers;

// Can add authorize attribute to controllers.



public class CustomerController : Controller
{

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> SelectAccount()
    {
        var accountList = new List<Account>();

        using (var httpClient = new HttpClient())
        {
            var response = httpClient.GetStringAsync("https://localhost:7130/api/Accounts").Result;

            accountList = JsonConvert.DeserializeObject<List<Account>>(response);

            if (accountList == null)
            {
                Console.WriteLine("didnt convert");
            }
        }
        AccountsViewModel model = new AccountsViewModel();
        model.Accounts = accountList;

        return View(model);
    }

    public async Task<IActionResult> AccountTransactions(int Account)
    {
        var account = new Account();

        using (var httpClient = new HttpClient())
        {
            var response = httpClient.GetStringAsync("https://localhost:7130/api/Accounts/" + Account).Result;

            account = JsonConvert.DeserializeObject<Account>(response);

            if (account == null)
            {
                Console.WriteLine("didnt convert");
            }
        }
        return View(account);
    }

    public async Task<IActionResult> SelectCustomer()
    {
       
        var customerList = new List<Customer>();

        CustomerView customerModel = new CustomerView();

        using (var httpClient = new HttpClient())
        {
            var response = httpClient.GetStringAsync("https://localhost:7130/api/Customers").Result;

            customerList = JsonConvert.DeserializeObject<List<Customer>>(response);

            if (customerList == null)
            {
                Console.WriteLine("didnt convert");
            }
            customerModel.Customers = customerList;
        }
        return View(customerModel);
    }

    public async Task<IActionResult> CustomerDetails(int CustomerId)
    {
        var customer = new Customer();

        using (var httpClient = new HttpClient())
        {
            var response = httpClient.GetStringAsync("https://localhost:7130/api/Customers/" + CustomerId).Result;
     
            customer = JsonConvert.DeserializeObject<Customer>(response);
                
        }
        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> EditCustomer(int CustomerID, string Name, string Address, string Suburb, string PostCode, string State, string Mobile, string TFN)
    {
        var customer = new Customer
        {
            CustomerID = CustomerID,
            Name = Name,
            Address = Address,
            Suburb = Suburb,
            PostCode = PostCode,
            State = State,
            Mobile = Mobile,
            TFN = TFN
        };

        using (var httpClient = new HttpClient())
        {
            StringContent updatedCustomer = new(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            await httpClient.PutAsync("https://localhost:7130/api/Customers/" + CustomerID, updatedCustomer);
        }
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> SelectLogin()
    {
        LoginViewModel loginModel = new LoginViewModel();

        var loginList = new List<Login>();

        using (var httpClient = new HttpClient())
        {
            var response = httpClient.GetStringAsync("https://localhost:7130/api/Logins").Result;

            loginList = JsonConvert.DeserializeObject<List<Login>>(response);

            if (loginList == null)
            {
                Console.WriteLine("didnt convert");
            }
        }
        Console.WriteLine(loginList.Count);
        loginModel.Logins = loginList;
        Console.WriteLine(loginModel.Logins.Count);
        return View(loginModel);
    }

    public async Task<IActionResult> LockAccount(string id)
    {

        using (var httpClient = new HttpClient())
        {
            StringContent sendLoginID = new(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");
            await httpClient.GetAsync("https://localhost:7130/api/Logins/" + id);
        }

        return RedirectToAction("Index");

    }

    public async Task<IActionResult> SelectBillPay()
    {
        BillPayView billModel = new BillPayView();

        var billpayList = new List<BillPay>();

        using (var httpClient = new HttpClient())
        {
            var response = httpClient.GetStringAsync("https://localhost:7130/api/BillPays").Result;

            billpayList = JsonConvert.DeserializeObject<List<BillPay>>(response);

            if (billpayList == null)
            {
                Console.WriteLine("didnt convert");
            }
        }
        billModel.BillPays = billpayList;

        return View(billModel);
    }

    public async Task<IActionResult> UpdateBillPay(int id)
    {
        using (var httpClient = new HttpClient())
        {
            StringContent sendBillPayID = new(JsonConvert.SerializeObject(id), Encoding.UTF8, "application/json");
            await httpClient.GetAsync("https://localhost:7130/api/BillPays/" + id);
        }

        return RedirectToAction("Index");
    }
}

