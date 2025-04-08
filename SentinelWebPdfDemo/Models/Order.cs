namespace SentinelWebPdfDemo.Models;

public class Order
{
    public int OrderID { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public int TotalCost { get; set; }
    public int CustomerID { get; set; }
}
