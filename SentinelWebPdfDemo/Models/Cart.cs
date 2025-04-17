namespace SentinelWebPdfDemo.Models;

public class Cart
{
    public List<CartItem> CartItems { get; set; } = new();

    public void AddItem(CartItem cartItem)
    {
        CartItems.Add(cartItem);
    }

    public void AddItem(Product product, int quantity)
    {
        CartItems.Add(new(product, quantity));
    }

    public void RemoveItem(int itemIdx)
    {
        CartItems.RemoveAt(itemIdx);
    }

    public void EmptyCart() => CartItems.Clear();
}
