namespace Core.Entities.OrderAggregate;

public class ProductItemOrdered
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public string PectureUrl { get; set; } = default!;
}