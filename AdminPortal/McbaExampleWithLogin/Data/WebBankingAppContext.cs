using Microsoft.EntityFrameworkCore;
using WebBankingApp.Models;

namespace WebBankingApp.Data;

public class WebBankingAppContext : DbContext
{
    public WebBankingAppContext(DbContextOptions<WebBankingAppContext> options) : base(options)
    { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BillPay> BillPays { get; set; }
    public DbSet<Payee> Payees { get; set; }

    // Fluent-API.
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Account>().HasOne(a => a.Customer).WithMany(c => c.Accounts);
        builder.Entity<BillPay>().HasOne(b => b.Account).WithMany(a => a.BillPays);
        builder.Entity<Transaction>().HasOne(t => t.Account).WithMany(a => a.Transactions).HasForeignKey(t => t.AccountNumber);
        builder.Entity <BillPay>().HasOne(b => b.Payee).WithMany(p => p.BillPays);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("WebBankingAppContext"));
    }

}
