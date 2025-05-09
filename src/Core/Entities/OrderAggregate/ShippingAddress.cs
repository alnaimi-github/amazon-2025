namespace Core.Entities.OrderAggregate;

public class ShippingAddress
{
public string Name { get; set; }= default!;
public string Line1 { get; set; } = default!;
public string? Line2 { get; set; }
public string City { get; set; } = default!;
public string State { get; set; } = default!;
public string PostalCode { get; set; } = default!;
public string Country { get; set; } = default!;
}