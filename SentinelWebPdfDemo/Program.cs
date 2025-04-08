using Microsoft.Data.SqlClient;
using SentinelWebPdfDemo.Components;
using SentinelWebPdfDemo.Models;
using SentinelWebPdfDemo.Services;

namespace SentinelWebPdfDemo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        //add my own services
        builder.Services.AddSingleton<IConfiguration>(provider =>
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json");

            return configBuilder.Build();
        });

        builder.Services.AddSingleton<Func<SqlConnection>>(provider => {
            var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("Default");
            return () => new SqlConnection(connectionString);
        });

        builder.Services.AddSingleton<OrdersRepository>();
        builder.Services.AddSingleton<ProductRepository>();
        builder.Services.AddSingleton<Cart>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();
   

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}
