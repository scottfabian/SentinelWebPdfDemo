namespace SentinelWebPdfDemo.Models;

public class Product
{
    public string ProductSku { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public bool IsEnterprise { get; set; }
    public int ListPrice { get; set; }
    public string ImageName { get; set; }
}
