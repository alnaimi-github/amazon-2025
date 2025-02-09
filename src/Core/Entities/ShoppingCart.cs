namespace Core.Entities;

public sealed class ShoppingCart
{
    public string Id { get; set; } = default!;
    public List<CartItem> Items { get; set; } = [];
    public int? DeliveryMethodId { get; set; }
    public string? ClientSecret { get; set; }
    public string? PaymentIntentId { get; set; }
}
