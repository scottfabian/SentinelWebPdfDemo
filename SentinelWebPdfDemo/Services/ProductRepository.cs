using Dapper;
using Microsoft.Data.SqlClient;
using SentinelWebPdfDemo.Models;

namespace SentinelWebPdfDemo.Services;

public class ProductRepository
{
    private readonly Func<SqlConnection> _createConnection;
    public List<Product> Products;

    public ProductRepository(Func<SqlConnection> connFactory)
    {
        _createConnection = connFactory;
    }

    public async Task<IEnumerable<T>> QueryDatabase<T>(string query)
    {
        IEnumerable<T> dbResult;

        using (var sqlConn = _createConnection())
        {
            await sqlConn.OpenAsync();

            dbResult = await sqlConn.QueryAsync<T>(query);

            await sqlConn.CloseAsync();
        }

        return dbResult;
    } 

    public async Task<List<Product>> GetAllProducts()
    {
        string query = "SELECT [ProductSku], [ProductName], [ProductDescription], [IsEnterprise], [ListPrice], [ImageName] FROM [SentinelWebPdf].[dbo].[Products]";

        Products = (await QueryDatabase<Product>(query)).ToList();

        return Products;
    }

    public async Task<Product?> GetProductBySkuAsync(string sku)
    {
        Product? targetProduct = Products.Where(x => x.ProductSku == sku).FirstOrDefault();

        if (targetProduct is not null)
        {
            return targetProduct;
        }

        string query = $"SELECT [ProductSku], [ProductName], [ProductDescription], [IsEnterprise], [ListPrice], [ImageName] FROM [SentinelWebPdf].[dbo].[Products] WHERE ProductSku = '{sku}'";

        Product? product = (await QueryDatabase<Product>(query)).FirstOrDefault();

        if (product is not null)
        {
            Products.Add(product);
        }

        return product;
    }

    public Product? GetProductBySku(string sku) => Products.Where(x => x.ProductSku == sku).FirstOrDefault();
}
