namespace SentinelWebPdfDemo.Models;

public class OrderWithDetails
{
    public int              OrderId             { get; set; }
    public string?          OrderNumber         { get; set; }
    public DateTime         OrderDate           { get; set; }
    public decimal          TotalCost           { get; set; }
    public string?          SkuList             { get; set; }

    public OrderWithDetails()
    {
        
    }

    

    public OrderWithDetails(int orderId, string orderNumber, DateTime orderDate, decimal totalCost, string skuList)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        OrderDate = orderDate;
        TotalCost = totalCost;
        SkuList = skuList;
    }

}
