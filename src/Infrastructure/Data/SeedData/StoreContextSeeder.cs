using System;
using System.Text.Json;

namespace infrastructure.Data.SeedData;

public static class StoreContextSeeder
{
    public static async Task SeedAsync(StoreContext storeContext)
    {
        if (!storeContext.Products.Any())
        {
           
           var products =  await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
              var productsList = JsonSerializer.Deserialize<List<Product>>(products);
                if (productsList == null) return;
                  
                  storeContext.Products.AddRange(productsList);
                      await storeContext.SaveChangesAsync();
                
        }
    }

}
