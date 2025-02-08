namespace Infrastructure.Data.SeedData;

public static class StoreContextSeeder
{
    public static async Task SeedAsync(StoreContext storeContext, ILogger logger)
    {
        if (storeContext == null)
            throw new ArgumentNullException(nameof(storeContext));

        await SeedEntitiesAsync<Product>(
            storeContext,
            storeContext.Products,
            Path.Combine("../Infrastructure/Data/SeedData/products.json"),
            logger).ConfigureAwait(false);

        await SeedEntitiesAsync<DeliveryMethod>(
            storeContext,
            storeContext.DeliveryMethods,
            Path.Combine("../Infrastructure/Data/SeedData/delivery.json"),
            logger).ConfigureAwait(false);
    }

    private static async Task SeedEntitiesAsync<TEntity>(
        StoreContext context,
        DbSet<TEntity> dbSet,
        string relativePath,
        ILogger logger) where TEntity : class
    {
        try
        {
            if (await dbSet.AnyAsync().ConfigureAwait(false))
            {
                logger.LogInformation($"Database already contains {typeof(TEntity).Name} data. Skipping seeding.");
                return;
            }

            var fullPath = Path.Combine(relativePath);

            if (!File.Exists(fullPath))
            {
                logger.LogError($"Seed file not found: {fullPath}");
                return;
            }

            var entities = JsonSerializer.Deserialize<List<TEntity>>(
                await File.ReadAllTextAsync(fullPath).ConfigureAwait(false),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (entities == null)
            {
                logger.LogError($"Seed file is empty: {fullPath}");
                return;
            }

            await dbSet.AddRangeAsync(entities).ConfigureAwait(false);
        
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"An error occurred while seeding {typeof(TEntity).Name} data.");
        }
    }
}