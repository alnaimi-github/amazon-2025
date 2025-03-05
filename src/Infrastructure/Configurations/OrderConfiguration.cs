namespace Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Core.Entities.OrderAggregate.Order>
{
    public void Configure(EntityTypeBuilder<Core.Entities.OrderAggregate.Order> builder)
    {

        builder.OwnsOne(x => x.ShippingAddress, o=> o.WithOwner());
        builder.OwnsOne(x => x.PaymentSummary,  o=> o.WithOwner());
        builder.Property(x => x.Status).HasConversion(
        o => o.ToString(),
        o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o));

       builder.HasMany(x => x.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

       builder.Property(x => x.OrderDate).HasConversion(
         d => d.ToUniversalTime(),
         d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
    }
}