using WebBankingApp.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

namespace WebBankingApp.Data;

public static class SeedData
{
    public static decimal GenerateBalance(Account account)
    {
        decimal balance = 0;

        foreach (Transaction transaction in account.Transactions)
        {
            if (transaction.TransactionType == TransactionType.Deposit)
            {
                balance += transaction.Amount;
            }
            else
            {
                balance -= transaction.Amount;
            }
        }
        return balance;
    }

    // Method for TESTING
    public static void ClearData()
    {
        using var connection = new SqlConnection("");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText =
            "delete from Transactions";
        command.ExecuteNonQuery();

        command.CommandText =
            "delete from Accounts";
        command.ExecuteNonQuery();

        command.CommandText =
            "delete from Logins";
        command.ExecuteNonQuery();

        command.CommandText =
            "delete from Customers";
        command.ExecuteNonQuery();

        command.CommandText =
            "delete from BillPays";
        command.ExecuteNonQuery();

        command.CommandText =
            "delete from Payees";
        command.ExecuteNonQuery();
    }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        HttpClient client = new();
        string APIConnectionString = "";

        Console.WriteLine("API String = " + APIConnectionString);

        var context = serviceProvider.GetRequiredService<WebBankingAppContext>();

        // Look for customers.
        if (context.Customers.Any())
        {
            Console.WriteLine("Database Already Seeded with Data -- No Seeding Required");
            return;
        }
        // DB has already been seeded.

        var CustomerList = new List<Customer>();
        var LoginList = new List<Login>();
        var AccountsList = new List<Account>();
        var payeeList = new List<Payee>();

        var response = client.GetStringAsync(new Uri(APIConnectionString)).Result;

        if (response == null)
        {
            Console.Write("null response");
        }

        List<Root> myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(response);

        if (myDeserializedClass == null)
        {
            Console.WriteLine("didnt convert");
        }

        foreach (var item in myDeserializedClass)
        {
            var myCustomer = new Customer
            { CustomerID = item.CustomerID,
                Name = item.Name,
                Address = item.Address,
                Suburb = item.City,
                PostCode = item.PostCode };
            CustomerList.Add(myCustomer);

            var myLogin = new Login {
                LoginID = item.Login.LoginID,
                CustomerID = item.CustomerID,
                PasswordHash = item.Login.PasswordHash,
                Auth = Authorization.Authorized
            };
            LoginList.Add(myLogin);

            AccountsList.AddRange(item.Accounts);
        }

        foreach (Account account in AccountsList)
        {
            foreach (Transaction transaction in account.Transactions)
            {
                transaction.TransactionType = TransactionType.Deposit;
            }
        }

        foreach (Customer customer in CustomerList)
        {
            if (customer.PostCode == null)
            {
                customer.PostCode = "NULL";
            }

            else if (customer.Address == null)
            {
                customer.Address = "NULL";
            }

            else if (customer.Suburb == null)
            {
                customer.Suburb = "NULL";
            }

            context.Customers.Add(customer);
        }

        foreach (Account account in AccountsList)
        {
            account.Balance = GenerateBalance(account);
            context.Accounts.Add(account);
        }

        foreach (Login login in LoginList)
        {
            context.Logins.Add(login);
        }

        payeeList = GeneratePayees();

        foreach (Payee payee in payeeList)
        {
            context.Payees.Add(payee);
        }

        context.SaveChanges();
    }

    private static List<Payee> GeneratePayees()
    {
        List<Payee> payeeList = new();

        var payee1 = new Payee
        {
            Name = "Robert Pickton",
            Address = "23 Movie Drive",
            Suburb = "Mulberry",
            State = "VIC",
            Postcode = "3145",
            Phone = "06112345678901"
        };

        var payee2 = new Payee
        {
            Name = "Stuart Rockwell",
            Address = "12 Goulburn Road",
            Suburb = "Strawberry",
            State = "NSW",
            Postcode = "3521",
            Phone = "06114582789012"
        };

        var payee3 = new Payee
        {
            Name = "Robert Pickton",
            Address = "23 Movie Drive",
            Suburb = "Raspberry",
            State = "QLD",
            Postcode = "3145",
            Phone = "06112345678901"
        };

        payeeList.Add(payee1);
        payeeList.Add(payee2);
        payeeList.Add(payee3);

        return payeeList;
    }

    public static async Task CheckBillPays(IServiceProvider serviceProvider)
    {

        var context = serviceProvider.GetRequiredService<WebBankingAppContext>();
        List<BillPay> list = context.BillPays.ToList();
        DateTime currentTime = DateTime.Now;
        DateTime dt1 = new DateTime();
        foreach (BillPay bill in list)
        {
            var timing = DateTime.Compare(bill.ScheduleTimeUtc, currentTime);

            if (bill.Period == PeriodType.Blocked || bill.LastPaid != dt1)
            {
                continue;
            }
            if (timing < 0 && bill.Period != PeriodType.Blocked)
            {
                var account = context.Accounts.Find(bill.AccountNumber);

                var trans = new Transaction
                {
                    TransactionType = TransactionType.BillPay,
                    Amount = bill.Amount,
                    TransactionTimeUtc = DateTime.UtcNow,
                    AccountNumber = bill.AccountNumber,
                    Comment = bill.Payee.Name,
                };

                if (account.Balance - bill.Amount < 0)
                {
                    bill.Period = PeriodType.Failed;
                    context.SaveChanges();
                    continue;
                }
                else
                {
                    if (bill.Period == PeriodType.Monthly)
                    {
                        BillPay monthlyBill = new BillPay();
                        monthlyBill.Amount = bill.Amount;
                        monthlyBill.Period = PeriodType.Monthly;
                        monthlyBill.PayeeID = bill.PayeeID;
                        monthlyBill.AccountNumber = bill.AccountNumber;
                        monthlyBill.ScheduleTimeUtc = bill.ScheduleTimeUtc;

                        monthlyBill.ScheduleTimeUtc = monthlyBill.ScheduleTimeUtc.AddMonths(1);
                        Console.WriteLine(monthlyBill.ScheduleTimeUtc);
                        account.BillPays.Add(monthlyBill);
                    }
                    bill.Period = PeriodType.Paid;
                    account.Balance -= bill.Amount;
                    account.Transactions.Add(trans);
                    bill.LastPaid = DateTime.UtcNow;
                }
                context.SaveChanges();
            }
        }
    }
}
