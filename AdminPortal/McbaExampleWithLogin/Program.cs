using WebBankingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace WebBankingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Store session into Web-Server memory.
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                // Make the session cookie essential.
                options.Cookie.IsEssential = true;
            });

            // Bonus Material: Store session into SQL Server.
            // Please see session-commands.md file.
            // Package required: Microsoft.Extensions.Caching.SqlServer
            //builder.Services.AddDistributedSqlServerCache(options =>
            //{
            //    options.ConnectionString = builder.Configuration.GetConnectionString(nameof(McbaContext));
            //    options.SchemaName = "dotnet";
            //    options.TableName = "SessionCache";
            //});
            //builder.Services.AddSession(options =>
            //{
            //    // Make the session cookie essential.
            //    options.Cookie.IsEssential = true;
            //    options.IdleTimeout = TimeSpan.FromDays(7);
            //});

            builder.Services.AddControllersWithViews();

            // Bonus Material: Implement global authorisation check. Also see the AuthorizeCustomerAttribute.cs file.
            //builder.Services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeCustomerAttribute()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
                app.UseExceptionHandler("/Home/Error");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
