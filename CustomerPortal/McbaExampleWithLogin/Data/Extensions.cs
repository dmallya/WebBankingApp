using System.Data;
using Microsoft.Data.SqlClient;
using WebBankingApp.Models;

namespace WebBankingApp.Data;
public static class Extensions
{
    private static List<Transaction> TransactionList = new();
    public static DataTable GetDataTable(this SqlCommand command)
    {
        using var adapter = new SqlDataAdapter(command);

        var table = new DataTable();
        adapter.Fill(table);

        return table;
    }
}