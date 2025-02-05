using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace infrastructure.Data;

public class StoreContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Address> Addresss => Set<Address>();

      protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
  {
    configurationBuilder.Properties<decimal>()
      .HavePrecision(18, 6);
  }
}
