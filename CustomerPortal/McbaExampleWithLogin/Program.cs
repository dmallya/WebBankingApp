using WebBankingApp.Data;
using Microsoft.EntityFrameworkCore;

namespace WebBankingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<WebBankingAppContext>(options =>
            {
                // Enable lazy loading.
                options.UseLazyLoadingProxies();
            });

            // Store session into Web-Server memory.
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                // Make the session cookie essential.
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Seed data.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    //SeedData.ClearData();
                    SeedData.Initialize(services);
                    SeedData.CheckBillPays(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

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
