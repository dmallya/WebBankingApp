using Microsoft.AspNetCore.Mvc;
using WebBankingApp.Data;
using WebBankingApp.Models;
using WebBankingApp.Utilities;
using WebBankingApp.Filters;
using SimpleHashing;

namespace WebBankingApp.Controllers;

// Can add authorize attribute to controllers.
[AuthorizeCustomer]
public class CustomerController : Controller
{
    private readonly WebBankingAppContext _context;

    // ReSharper disable once PossibleInvalidOperationException
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public CustomerController(WebBankingAppContext context) => _context = context;

    public static Transaction CurrentTransaction { get; set; }

    public static Transaction CurrentTransferTransaction { get; set; }

    // Can add authorize attribute to actions.
    //[AuthorizeCustomer]
    public async Task<IActionResult> Index()
    {
        // Lazy loading.
        // The Customer.Accounts property will be lazy loaded upon demand.
        var customer = await _context.Customers.FindAsync(CustomerID);

        // OR
        // Eager loading.
        //var customer = await _context.Customers.Include(x => x.Accounts).
        //    FirstOrDefaultAsync(x => x.CustomerID == _customerID);

        return View(customer);
    }

    public async Task<IActionResult> Deposit()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        List<BillPay> list = _context.BillPays.ToList();

        Console.WriteLine(list.Count);

        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(int accountNumber, decimal amount, string Comment)
    {
        var account = await _context.Accounts.FindAsync(accountNumber);
        var customer = await _context.Customers.FindAsync(CustomerID);
        UtilityMethods utilities = new();
        account.Balance = utilities.GenerateBalance(_context, account);


        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(customer);
        }
        if (amount > 100000000000000)
        {
            ModelState.AddModelError(nameof(amount), "Amount is insanely large, you arent that rich");
            return View(customer);
        }
        if (account.Balance > 100000000000000)
        {
            ModelState.AddModelError(nameof(amount), "Your balance is full, enjoy your money or buy the bank to deposit more");
            return View(customer);
        }
        string addComment = "";
        if (Comment is not null)
        {
            if (Comment.Length > 30)
            {
                ModelState.AddModelError(nameof(Comment), "Comment To Large.");
                return View(customer);
            }
            else
            {
                addComment = Comment;
            }
        }

        var trans = new Transaction
        {
            TransactionType = TransactionType.Deposit,
            Amount = amount,
            Comment = addComment,
            TransactionTimeUtc = DateTime.UtcNow,
            AccountNumber = accountNumber,
        };

        CurrentTransaction = trans;

        return RedirectToAction(nameof(ReviewTransaction));
    }

    public async Task<IActionResult> ReviewTransaction()
    {
        return View(CurrentTransaction);
    }

    public async Task<IActionResult> ConfirmTransaction()
    {
        UtilityMethods utilities = new();
        var account = await _context.Accounts.FindAsync(CurrentTransaction.AccountNumber);
        Console.WriteLine(account.AccountNumber);
        
        Console.WriteLine("Current Account Balance before transaction: " + account.Balance);

        if (CurrentTransaction.TransactionType == TransactionType.Deposit)
        {
            account.Balance += CurrentTransaction.Amount;
            Console.WriteLine("Current Account Balance After Deposit: " + account.Balance);
            account.Transactions.Add(CurrentTransaction);
        }
        if (CurrentTransaction.TransactionType == TransactionType.Withdrawal)
        {
            account.Balance -= CurrentTransaction.Amount;
            Console.WriteLine("Current Account Balance After Withdrawal: " + account.Balance);
            account.Transactions.Add(CurrentTransaction);
            ServiceFee(utilities, account.AccountNumber, _context, account, (decimal)0.05);
        }
        if (CurrentTransaction.TransactionType == TransactionType.Transfer)
        {
            account.Balance -= CurrentTransaction.Amount;
            Console.WriteLine("Current Account Balance After Transfer: " + account.Balance);
            var transferAccount = await _context.Accounts.FindAsync(CurrentTransferTransaction.AccountNumber);
           
            Console.WriteLine("Transfered Account Balance Before Transfer: " + transferAccount.Balance);
            transferAccount.Balance += CurrentTransaction.Amount;
            Console.WriteLine("Transfered Account Balance after Transfer: " + transferAccount.Balance);
            account.Transactions.Add(CurrentTransaction);
            transferAccount.Transactions.Add(CurrentTransferTransaction);
            ServiceFee(utilities, account.AccountNumber, _context, account, (decimal)0.1);
        }
        static void ServiceFee(UtilityMethods utilities, int AccountNumber, WebBankingAppContext _context, Account account, decimal amount)
        {
            int numTransactions = utilities.GetNumTransactions(_context, AccountNumber);
            if (numTransactions > 1)
            {

                var serviceCharge = new Transaction
                {
                    TransactionType = TransactionType.Service,
                    Amount = (decimal)amount,
                    Comment = "Service Charge",
                    TransactionTimeUtc = DateTime.UtcNow,
                    AccountNumber = AccountNumber,
                    DestinationAccountNumber = null
                };
                account.Balance -= serviceCharge.Amount;
                _context.Transactions.Add(serviceCharge);
            }
        }
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Withdraw()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);
        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Withdraw(int Account, decimal amount, string Comment)
    {
        UtilityMethods utilities = new();

        var account = await _context.Accounts.FindAsync(Account);
        var customer = await _context.Customers.FindAsync(CustomerID);

        if (amount <= 0)
            ModelState.AddModelError(nameof(amount), "Amount must be positive.");
        if (amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(amount), "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = amount;
            return View(customer);
        }
        Decimal accountFromBalance = utilities.GenerateBalance(_context, account);
        if (amount > accountFromBalance)
        {
            ModelState.AddModelError(nameof(amount), "Incorrect Amount: Not enough balance");
            return View(customer);
        }

        string addComment = "";
        if (Comment is not null)
        {
            if (Comment.Length > 30)
            {
                ModelState.AddModelError(nameof(Comment), "Comment To Large.");
                return View(customer);
            }
            else
            {
                addComment = Comment;
            }
        }

        var trans = new Transaction
        {
            TransactionType = TransactionType.Withdrawal,
            Amount = amount,
            Comment = addComment,
            TransactionTimeUtc = DateTime.UtcNow,
            AccountNumber = Account,
        };
        CurrentTransaction = trans;

        return RedirectToAction(nameof(ReviewTransaction));
    }

    public async Task<IActionResult> MyStatement()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        await CheckBillPays();

        return View(customer);
    }

    public async Task<IActionResult> MyStatementView(int id, int startIndex)
    {
        var account = await _context.Accounts.FindAsync(id);
       
        StatementViewModel Statement = new StatementViewModel();

        Statement.Account = account;

        if (startIndex >= Statement.Account.Transactions.Count)
        {
            return View(Statement);
        }
        else
        {
            Statement.StartIndex = startIndex;
            return View(Statement);
        }
    }

    public async Task<IActionResult> Transfer()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Transfer(int AccountFrom, decimal Amount, int Account, string Comment)
    {
        UtilityMethods utilities = new();

        var customer = await _context.Customers.FindAsync(CustomerID);
        var myaccount = await _context.Accounts.FindAsync(AccountFrom);
        var transferedaccount = _context.Accounts.Find(Account);

        if (Amount <= 0)
            ModelState.AddModelError(nameof(Amount), "Amount must be positive.");
        if (Amount.HasMoreThanTwoDecimalPlaces())
            ModelState.AddModelError(nameof(Amount), "Amount cannot have more than 2 decimal places.");
        if (!ModelState.IsValid)
        {
            ViewBag.Amount = Amount;
            return View(customer);
        }

        if (transferedaccount == null)
        {
            ModelState.AddModelError(nameof(Account), "Account Not Found.");
            return View(customer);
        }
        if (transferedaccount == myaccount)
        {
            ModelState.AddModelError(nameof(AccountFrom), "Cannot Transfer to and from same account.");
            return View(customer);
        }

        string addComment = "";
        if (Comment is not null)
        {
            if (Comment.Length > 30)
            {
                ModelState.AddModelError(nameof(Comment), "Comment To Large.");
                return View(customer);
            }
            else
            {
                addComment = Comment;
            }
        }

        //Decimal accountFromBalance = utilities.GenerateBalance(_context, myaccount);
        if (Amount > myaccount.Balance)
        {
            ModelState.AddModelError(nameof(Amount), "Incorrect Amount: Not enough balance");
            Console.WriteLine(myaccount.Balance);
            return View(customer);
        }

        var mytrans = new Transaction
        {
            TransactionType = TransactionType.Transfer,
            Amount = Amount,
            Comment = addComment,
            TransactionTimeUtc = DateTime.UtcNow,
            AccountNumber = AccountFrom,
            DestinationAccountNumber = Account
        };

        var accountTrans = new Transaction
        {
            TransactionType = TransactionType.Transfer,
            Amount = Amount,
            Comment = addComment,
            TransactionTimeUtc = DateTime.UtcNow,
            AccountNumber = Account,
            DestinationAccountNumber = AccountFrom
        };
        CurrentTransaction = mytrans;
        CurrentTransferTransaction = accountTrans;

        return RedirectToAction(nameof(ReviewTransaction));
    }

    public async Task<IActionResult> CustomerDetails() => View(await _context.Customers.FindAsync(CustomerID));

    [HttpPost]
    public async Task<IActionResult> CustomerDetails(string Name, string Address, string Suburb, string PostCode, string State, string Mobile, string TFN)
    {
        var customer = await _context.Customers.FindAsync(CustomerID);
        customer.Name = Name;
        customer.Address = Address;
        customer.Suburb = Suburb;
        customer.PostCode = PostCode;
        customer.State = State;
        customer.Mobile = Mobile;
        customer.TFN = TFN;
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Password() => View();

    [HttpPost]
    public async Task<IActionResult> Password(string LoginID, string OldPassword, string NewPassword)
    {
        var login = await _context.Logins.FindAsync(LoginID);
        if (login == null)
        {
            ModelState.AddModelError("LoginFailed", "Login failed, Incorrect LoginID");
            return View();
        }
        if (!PBKDF2.Verify(login.PasswordHash, OldPassword))
        {
            ModelState.AddModelError("LoginFailed", "Login failed, Incorrect Password");
            return View();
        }
        login.PasswordHash = PBKDF2.Hash(NewPassword);
        await _context.SaveChangesAsync();
        return View();

    }

    public async Task<IActionResult> BillPay()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        return View(customer);
    }

    public async Task<IActionResult> BillPayView(int accountnumber)
    {
        var account = await _context.Accounts.FindAsync(accountnumber);

        await CheckBillPays();

        return View(account);
    }

    public async Task<IActionResult> CreateBillPay()
    {
        var customer = await _context.Customers.FindAsync(CustomerID);

        ViewModel myModel = new ViewModel();

        myModel.Customer = customer;
        myModel.Payees = _context.Payees.ToList();

        return View(myModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBillPayView(ViewModel model)
    {
        BillPay bill = model.BillPay;

        var account = await _context.Accounts.FindAsync(bill.AccountNumber);

        account.BillPays.Add(bill);

        await _context.SaveChangesAsync();

        return RedirectToAction("BillPayView", new { accountnumber = account.AccountNumber });
    }

    
    public async Task<IActionResult> EditBillPay(int id)

    {
        Console.WriteLine(id);   
        var bill = await _context.BillPays.FindAsync(id);

        return View(bill);
    }

    public async Task<IActionResult> UpdateBillPay(BillPay bill)
    {
        var updatedbill = await _context.BillPays.FindAsync(bill.BillPayId);
        updatedbill.Amount = bill.Amount;
        updatedbill.ScheduleTimeUtc = bill.ScheduleTimeUtc;
        updatedbill.Period = bill.Period;

        await _context.SaveChangesAsync();

        return RedirectToAction("BillPayView", new { accountnumber = updatedbill.AccountNumber });
    }

    public async Task<IActionResult> DeleteBillPay(int id)

    {
        var bill = await _context.BillPays.FindAsync(id);

        return View(bill);
    }

    public async Task<IActionResult> ConfirmDeleteBillPay(int BillPayId)

    {
        BillPay bill =  await _context.BillPays.FindAsync(BillPayId);

        _context.BillPays.Remove(bill);

        await _context.SaveChangesAsync();

        return RedirectToAction("BillPayView", new { accountnumber = bill.AccountNumber });
    }

    public async Task<IActionResult> Payee()
    {
        return View();
    }

    public async Task<IActionResult> AddPayee(Payee newPayee)
    {
        _context.Payees.Add(newPayee);
        await _context.SaveChangesAsync();
        return RedirectToAction("CreateBillPay");
    }

    public async Task CheckBillPays()
    {
        List<BillPay> list = _context.BillPays.ToList();
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
                var account = _context.Accounts.Find(bill.AccountNumber);

                var trans = new Transaction
                {
                    TransactionType = TransactionType.BillPay,
                    Amount = bill.Amount,
                    TransactionTimeUtc = bill.ScheduleTimeUtc,
                    AccountNumber = bill.AccountNumber,
                    Comment = bill.Payee.Name,
                };

                if (account.Balance - bill.Amount < 0)
                {
                    bill.Period = PeriodType.Failed;
                    _context.SaveChanges();
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
                    bill.LastPaid = bill.ScheduleTimeUtc;
                }
                _context.SaveChanges();
            }
        }
    }
}

