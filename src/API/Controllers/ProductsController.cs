using Core.Entities;
using Core.Intarfaces;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductRepository productRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProductsAsync(string? brand,string? type,
     CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsAsync(brand,type,cancellationToken);
        return Ok(products);
    }

   [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductAsync(int id,
     CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProductByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found." });
        }

        return Ok(product);
    }

   [HttpPost]
public async Task<IActionResult> CreateProductAsync(Product product, CancellationToken cancellationToken)
{
    if (product == null)
    {
        return BadRequest(new { Message = "Product data is invalid." });
    }

    productRepository.AddProduct(product);

    if (await productRepository.SaveChangesAsync(cancellationToken))
    {
        return Ok(product);
    }

    return StatusCode(500, new { Message = "An error occurred while saving the product." });
}


    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProductAsync(int id, Product product,
     CancellationToken cancellationToken)
    {
        if (id != product.Id)
        {
            return BadRequest(new { Message = "Product ID mismatch." });
        }

        if (!productRepository.ProductExists(id))
        {
            return NotFound(new { Message = $"Product with ID {id} not found." });
        }

        productRepository.UpdateProduct(product);

        if (!await productRepository.SaveChangesAsync(cancellationToken))
        {
            return StatusCode(500, new { Message = "An error occurred while saving the product." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProductAsync(int id, 
    CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProductByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found." });
        }

        productRepository.DeleteProduct(product);
        if (!await productRepository.SaveChangesAsync(cancellationToken))
        {
            return StatusCode(500, new { Message = "An error occurred while deleting the product." });
        }

        return NoContent();
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrandsAsync(CancellationToken cancellationToken)
    {
        var brands = await productRepository.GetBrandsAsync(cancellationToken);
        return Ok(brands);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetTypesAsync(CancellationToken cancellationToken)
    {
        var types = await productRepository.GetTypesAsync(cancellationToken);
        return Ok(types);
    }
}