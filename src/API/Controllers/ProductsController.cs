using Core.Entities;
using Core.Intarfaces;
using Core.Specifications;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IGenericRepository<Product> repository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProductsAsync(string? brand,string? type,
     string? sort, CancellationToken cancellationToken)
    {
        var spec = new ProductSpecification(brand, type, sort);
        var products = await repository.ListSpecificationAsync(spec, cancellationToken);
        return Ok(products);
    }

   [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductAsync(int id,
     CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
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

    await repository.AddAsync(product, cancellationToken);

    if (await repository.SaveChangesAsync(cancellationToken))
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

        if (!await repository.EntityExists(id, cancellationToken))
        {
            return NotFound(new { Message = $"Product with ID {id} not found." });
        }

       await  repository.UpdateAsync(product, cancellationToken);

        if (!await repository.SaveChangesAsync(cancellationToken))
        {
            return StatusCode(500, new { Message = "An error occurred while saving the product." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProductAsync(int id, 
    CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound(new { Message = $"Product with ID {id} not found." });
        }

        await repository.DeleteAsync(product, cancellationToken);
        if (!await repository.SaveChangesAsync(cancellationToken))
        {
            return StatusCode(500, new { Message = "An error occurred while deleting the product." });
        }

        return NoContent();
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrandsAsync(CancellationToken cancellationToken)
    {
       var spec = new BrandListSpecification();
        var brands = await repository.ListSpecAsync(spec,cancellationToken);
        return Ok(brands);
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetTypesAsync(CancellationToken cancellationToken)
    {
        var spec = new TypeListSpecification();
        var types = await repository.ListSpecAsync(spec,cancellationToken);
        return Ok(types);
    }
}