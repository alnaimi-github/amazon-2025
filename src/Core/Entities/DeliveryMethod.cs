namespace Core.Entities;

public sealed class DeliveryMethod : BaseEntity
{
    public string ShortName { get; set; } = default!;
    public string DeliveryTime { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
}