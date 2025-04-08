using SentinelWebPdfDemo.Models;
using SentinelWebPdfDemo.Services;
using Microsoft.Data.SqlClient;
using Dapper;


namespace ConsoleTester;

internal class Program
{
    static void Main(string[] args)
    {
        string connString = "Data Source=(local);Database=SentinelWebPdf;Integrated Security=false;User ID=sa;Password=TeklynxAdmin!;TrustServerCertificate=true;";

        Func<SqlConnection> sqlFactory = () =>
        {
            return new SqlConnection(connString);
        };

        var productRepo = new ProductRepository(sqlFactory);

        var products = productRepo.GetAllProducts().GetAwaiter().GetResult();
    }
}
