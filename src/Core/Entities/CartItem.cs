namespace Core.Entities;

public sealed class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; } = default!;
    public string Brand { get; set; } = default!;
    public string Type { get; set; } = default!;
}