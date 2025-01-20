namespace infrastructure.Data.Repositories;

public class ProductRepository(StoreContext storeContext) : IProductRepository
{


    public void AddProduct(Product product)
    {
        storeContext.Products.Add(product);
    }

    public void DeleteProduct(Product product)
    {
        storeContext.Products.Remove(product);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync(CancellationToken cancellationToken)
    {
        return await storeContext.Products.Select(p => p.Brand).Distinct().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync(CancellationToken cancellationToken)
    {
        return await storeContext.Products.Select(p => p.Type).Distinct().ToListAsync(cancellationToken);
    }
    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await storeContext.Products.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand,string? type,
    CancellationToken cancellationToken)
    {
        var products = storeContext.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(brand))
        {
            products = products.Where(p => p.Brand == brand);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            products = products.Where(p => p.Type == type);
        }

        return await products.ToListAsync(cancellationToken);
    }

    public bool ProductExists(int id)
    {
        return storeContext.Products.Any(e => e.Id == id);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await storeContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public void UpdateProduct(Product product)
    {
        storeContext.Entry(product).State = EntityState.Modified;
    }
}
