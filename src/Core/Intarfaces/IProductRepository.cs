namespace Core.Intarfaces;

public interface IProductRepository
{
   Task<IReadOnlyList<Product>> GetProductsAsync(string? brand,
   string? type,string? sort,CancellationToken cancellationToken);
   Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
   Task<IReadOnlyList<string>> GetBrandsAsync(CancellationToken cancellationToken);
   Task<IReadOnlyList<string>> GetTypesAsync(CancellationToken cancellationToken);
   void AddProduct(Product product);
   void UpdateProduct(Product product);
   void DeleteProduct(Product product);
   bool ProductExists(int id);
   Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
   
}
