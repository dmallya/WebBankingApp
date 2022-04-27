using WebBankingApp.Models;

namespace WebBankingApp.Data;

public class UtilityMethods
{
    public decimal GenerateBalance(WebBankingAppContext context, Account account)
    {
        decimal balance = 0;
        List<Transaction> transactionList = context.Transactions.ToList();

        Console.WriteLine(account.AccountNumber);

        foreach (Transaction transaction in transactionList)
        {
            if (transaction.AccountNumber == account.AccountNumber)
            {
                if (transaction.TransactionType == TransactionType.Deposit)
                {
                    balance += transaction.Amount;
                }
                else if(transaction.TransactionType == TransactionType.Transfer && transaction.DestinationAccountNumber == null)
                {
                    balance += transaction.Amount;
                }
                else
                {
                    balance -= transaction.Amount;
                }
            }
        }
        return balance;
    }

    public int GetNumTransactions(WebBankingAppContext context, int accountNumber)
    {
        int numTransactions = 0;
        List<Transaction> transactionList = context.Transactions.ToList();
        foreach(Transaction transaction in transactionList)
        {
            if (transaction.AccountNumber == accountNumber && (transaction.TransactionType == TransactionType.Transfer || transaction.TransactionType == TransactionType.Withdrawal))
            {
                numTransactions++;
            }
        }
        return numTransactions;
    }
}
