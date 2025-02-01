namespace Core.Entities;

public sealed class ShoppingCart
{
    public string Id { get; set; } = default!;
    public List<CartItem> Items { get; set; } = [];
}
