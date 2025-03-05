namespace infrastructure.Data;

public class StoreContext : IdentityDbContext<AppUser>
{
    public StoreContext(DbContextOptions<StoreContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Address> Addresss => Set<Address>();
    public DbSet<DeliveryMethod> DeliveryMethods => Set<DeliveryMethod>();
    public DbSet<Core.Entities.OrderAggregate.Order> Orders => Set<Core.Entities.OrderAggregate.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreContext).Assembly);

      base.OnModelCreating(modelBuilder);
  }
    protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
   {
          configurationBuilder.Properties<decimal>()
                              .HavePrecision(18, 6);
   }
 
}
