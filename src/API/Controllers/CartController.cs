namespace API.Controllers;

public class CartController(ICartService cartService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult> GetCartByIdAsync([FromQuery]string id)
    {
        var cart = await cartService.GetCartAsync(id); 
        return Ok(cart ?? new ShoppingCart { Id = id });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCartAsync(ShoppingCart cart)
    {
      var updatedCart = await cartService.SetCartAsync(cart);

      if(updatedCart is null) return BadRequest("Prblom with cart!");

      return Ok(updatedCart);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCartAsync(string id)
    {
       var deleted = await cartService.DeleteCartAsync(id);
       if(!deleted) return BadRequest("Prblom deleting cart!");

       return Ok();
    }
    
}