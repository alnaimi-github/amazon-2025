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

    protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
  {
          configurationBuilder.Properties<decimal>()
                              .HavePrecision(18, 6);
  }
}
