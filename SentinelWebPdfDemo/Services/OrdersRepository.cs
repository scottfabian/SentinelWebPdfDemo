using Dapper;
using Microsoft.Data.SqlClient;
using SentinelWebPdfDemo.Models;

namespace SentinelWebPdfDemo.Services;

public class OrdersRepository
{
    private readonly Func<SqlConnection> _createConnection;

    public List<Order> OrderList { get; set; }
    public List<OrderWithDetails> OrderListWithLines { get; set; }

    public OrdersRepository()
    {
        OrderListWithLines = new()
        {
            new OrderWithDetails() {OrderId = 1, OrderNumber = "123ABC", TotalCost = 420.69M, OrderDate = DateTime.Today, SkuList = "Sku1, Sku2"},
            new OrderWithDetails() {OrderId = 2, OrderNumber = "456DEF", TotalCost = 69.69M, OrderDate = DateTime.Today.AddDays(-7), SkuList = "Sku3, Sku4"},
            new OrderWithDetails() {OrderId = 3, OrderNumber = "789GHI", TotalCost = 14.14M, OrderDate = DateTime.Today.AddDays(-14), SkuList = "Sku5, Sku6"}
        };
    }

    public OrdersRepository(Func<SqlConnection> connectionFactory)
    {
        _createConnection = connectionFactory;

        InitializeData();
    }

    public void InitializeData()
    {
        Task.Run(async () => await GetAllOrders()).GetAwaiter().GetResult();
        Task.Run(async () => await GetAllOrderDetails()).GetAwaiter().GetResult();
    }

    #region CREATE

    public async Task InsertOrder(Order orderToInsert, string[] skuList)
    {
        string insertOrder = "INSERT INTO [Orders] (OrderNumber, OrderDate, TotalCost, CustomerID) VALUES(@OrderNumber, @OrderDate, @TotalCost, @CustomerID)";

        using (var sqlConn = _createConnection())
        {
            await sqlConn.OpenAsync();

            await sqlConn.QueryAsync(insertOrder, orderToInsert);

            for (int i = 0; i < skuList.Length; i++)
            {
                string insertOrderDetails = $"INSERT INTO [OrderDetails] (OrderNumber, ProductSku) VALUES ('{orderToInsert.OrderNumber}', '{skuList[i]}')";

                await sqlConn.QueryAsync(insertOrderDetails);
            }

            await sqlConn.CloseAsync();
        }
    }

    #endregion CREATE


    #region READ

    //generic db query
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


    public async Task<List<Order>> GetAllOrders()
    {
        string query = "SELECT [OrderID], [OrderNumber], [OrderDate], [TotalCost], [CustomerID] FROM [SentinelWebPdf].[dbo].[Orders]";

        OrderList = (await QueryDatabase<Order>(query)).ToList();

        return OrderList;
    }

    public async Task<List<OrderWithDetails>> GetAllOrderDetails()
    {
        string query = $@"SELECT 
                            o.OrderID, 
                            o.OrderNumber, 
                            OrderDate, 
                            TotalCost, 
                            STRING_AGG(p.ProductSku, ', ') AS SkuList
                        FROM Orders o
                        JOIN OrderDetails od
                            ON o.OrderNumber = od.OrderNumber
                        JOIN Products p
                            ON od.ProductSku = p.ProductSku
                        GROUP BY 
                            o.OrderID, 
                            o.OrderNumber, 
                            OrderDate, 
                            TotalCost";


        OrderListWithLines = (await QueryDatabase<OrderWithDetails>(query)).ToList();

        return OrderListWithLines;
    }

    public async Task<Order?> GetOrderByNumber(string orderNumber)
    {
        Order? targetOrder = OrderList.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();

        if (targetOrder is not null)
        {
            return targetOrder;
        }

        string query = $"SELECT [OrderID], [OrderNumber], [OrderDate], [TotalCost], [CustomerID] FROM [SentinelWebPdf].[dbo].[Orders] WHERE [OrderNumber] = '{orderNumber}'";


        targetOrder = (await QueryDatabase<Order>(query)).FirstOrDefault();
        

        if (targetOrder is not null)
        {
            OrderList.Add(targetOrder);
        }

        return targetOrder;
    }

    public async Task<OrderWithDetails?> GetOrderDetailsByNumber(string orderNumber)
    {

        OrderWithDetails? targetOrder = OrderListWithLines.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();

        if (targetOrder is not null)
        {
            return targetOrder;
        }

        string query = $@"SELECT 
                            o.OrderID, 
                            o.OrderNumber, 
                            OrderDate, 
                            TotalCost, 
                            STRING_AGG(p.ProductSku, ', ') AS SkuList
                        FROM Orders o
                        JOIN OrderDetails od
                            ON o.OrderNumber = od.OrderNumber
                        JOIN Products p
                            ON od.ProductSku = p.ProductSku
                        WHERE o.OrderNumber = '{orderNumber}'
                        GROUP BY 
                            o.OrderID, 
                            o.OrderNumber, 
                            OrderDate, 
                            TotalCost";

        targetOrder = (await QueryDatabase<OrderWithDetails>(query)).FirstOrDefault();

        if (targetOrder is not null)
        {
            OrderListWithLines.Add(targetOrder);
        }


        return targetOrder;
    }

    #endregion READ


    #region UPDATE

    #endregion UPDATE


    #region DELETE
    public async Task DeleteOrderByNumberAsync(string orderNumber)
    {
        string deleteDetailsQuery = $"DELETE FROM OrderDetails WHERE OrderNumber = '{orderNumber}'";
        string deleteOrdersQuery = $"DELETE FROM Orders WHERE OrderNumber = '{orderNumber}'";

        using (var sqlConn = _createConnection())
        {
            await sqlConn.OpenAsync();

            await sqlConn.QueryAsync(deleteDetailsQuery);

            await sqlConn.QueryAsync(deleteOrdersQuery);

            await sqlConn.CloseAsync();
        }
    }

    public async Task DeleteAllOrdersAsync()
    {
        string deleteDetailsQuery = "DELETE FROM OrderDetails";
        string deleteOrdersQuery = "DELETE FROM Orders";

        using (var sqlConn = _createConnection())
        {
            await sqlConn.OpenAsync();

            await sqlConn.QueryAsync(deleteDetailsQuery);

            await sqlConn.QueryAsync(deleteOrdersQuery);

            await sqlConn.CloseAsync();
        }
    }
    #endregion DELETE

}
