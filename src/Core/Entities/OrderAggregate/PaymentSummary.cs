namespace Core.Entities.OrderAggregate;

public class PaymentSummary
{
  public int Last4 { get; set; }
  public string Brand { get; set; } = default!;
  public int ExpMonth { get; set; }
  public int ExpYear { get; set; }
}