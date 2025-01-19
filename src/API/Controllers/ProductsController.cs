using Core.Entities;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(StoreContext storeContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await storeContext.Products.ToListAsync(cancellationToken);
       return Ok(products);
    }

   [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductAsync(int id,
     CancellationToken cancellationToken)
    {
        var product = await FindProductByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found." });
        }

        return Ok(product);
    }

   [HttpPost]
    public async Task<IActionResult> CreateProductAsync(Product product,
     CancellationToken cancellationToken)
    {
        if (product == null)
        {
            return BadRequest(new { Message = "Invalid product data." });
        }

        await storeContext.Products.AddAsync(product, cancellationToken);
        await storeContext.SaveChangesAsync(cancellationToken);

        return Ok(product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProductAsync(int id, Product product,
     CancellationToken cancellationToken)
    {
        if (product == null || id != product.Id)
        {
            return BadRequest(new { Message = "Product data is invalid or mismatched." });
        }

        var existingProduct = await FindProductByIdAsync(id, cancellationToken);
        if (existingProduct == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found." });
        }

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;

        await storeContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProductAsync(int id, 
    CancellationToken cancellationToken)
    {
        var product = await FindProductByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found." });
        }

        storeContext.Products.Remove(product);
        await storeContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

     private async Task<Product?> FindProductByIdAsync(int id, 
     CancellationToken cancellationToken)
    {
        return await storeContext.Products.FindAsync([id], cancellationToken);
    }
}