namespace Core.Entities.OrderAggregate;

public class Order : BaseEntity
{
   public DateTime OrderDate { get; set; } = DateTime.UtcNow;
   public string BuyerEmail { get; set; } = default!;
   public DeliveryMethod DeliveryMethod { get; set; } = null!;
   public ShippingAddress ShippingAddress { get; set; } = null!;
   public PaymentSummary PaymentSummary { get; set; } = null!;
   public List<OrderItem> OrderItems { get; set; } = [];
   public decimal Subtotal { get; set; }
   public OrderStatus Status { get; set; } = OrderStatus.Pending;
   public string PaymentIntentId { get; set; } = default!;

   public decimal GetTotal()
   {
      return Subtotal + DeliveryMethod.Price;
   }
}