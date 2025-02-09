using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

public class PaymentsController(
 IPaymentService paymentService,
 IGenericRepository<DeliveryMethod> dmRepo) 
 : BaseApiController
{
   [Authorize] 
  [HttpPost("{cartId}")]
  public async Task<IActionResult> CreateOrUpdatePayementIntentAsync(string cartid, 
  CancellationToken cancellationToken)
  {
     var cart = await paymentService.CreateOrUpdatePaymentIntent(cartid, cancellationToken);
     if( cart is null) return BadRequest("Problem with your cart");

     return Ok(cart);
  }

  [HttpGet("delivery-methods")]
  public async Task<IActionResult> GetDeliveryMethodsAsync(CancellationToken cancellationToken) 
  {
   return Ok(await dmRepo.ListAllAsync(cancellationToken));
  }
}