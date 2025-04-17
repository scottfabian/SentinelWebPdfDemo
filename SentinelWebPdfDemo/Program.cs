using Microsoft.Data.SqlClient;
using FabToolKit.Tkx.Sentinel.Rest;
using SentinelWebPdfDemo.Components;
using SentinelWebPdfDemo.Models;
using SentinelWebPdfDemo.Services;
using Microsoft.AspNetCore.Routing.Constraints;

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

        //sentinel related services
        builder.Services.AddSingleton<HttpClient>(provider =>
        {
            var socketHandler = new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10)
            };

            return new HttpClient(socketHandler);
        });
        builder.Services.AddSingleton<SentinelApiFactory>();

        //database related services
        builder.Services.AddSingleton<Func<SqlConnection>>(provider => {
            var connectionString = provider.GetRequiredService<IConfiguration>().GetConnectionString("Default");
            return () => new SqlConnection(connectionString);
        });
        builder.Services.AddSingleton<OrdersRepository>();
        builder.Services.AddSingleton<ProductRepository>();

        //site/general/misc services
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


        //map any APIs
        app.MapGet("/api/pdf/{filename}", async (string filename, HttpContext context) =>
        {
            var config = app.Services.GetRequiredService<IConfiguration>();

            string basePdfFileDirectory = config["PDFContext:SentinelOutputDirectory"]!;

            string fullFilePath = Path.Combine(basePdfFileDirectory, filename);

            if (!File.Exists(fullFilePath))
            {
                return Results.NotFound();
            }

            context.Response.ContentType = "application/pdf";
            
            await context.Response.SendFileAsync(fullFilePath);

            return Results.Empty;
        });


        app.Run();
    }
}
