using Core.Entities.OrderAggregate;
using Microsoft.AspNetCore.Authorization;
using Stripe;

namespace API.Controllers;

public class PaymentsController(
 IPaymentService paymentService,
 IUnitOfWork unit,
 ILogger<PaymentsController> logger)
 : BaseApiController
{

  private readonly string _whSecret = "";

  [Authorize]
  [HttpPost("{cartId}")]
  public async Task<IActionResult> CreateOrUpdatePayementIntentAsync(string cartid,
  CancellationToken cancellationToken)
  {
    var cart = await paymentService.CreateOrUpdatePaymentIntent(cartid, cancellationToken);
    if (cart is null) return BadRequest("Problem with your cart");

    return Ok(cart);
  }

  [HttpGet("delivery-methods")]
  public async Task<IActionResult> GetDeliveryMethodsAsync(CancellationToken cancellationToken)
  {
    return Ok(await unit.Repository<DeliveryMethod>().ListAllAsync(cancellationToken));
  }

  [HttpPost("webhook")]
  public async Task<IActionResult> StripeWebhook()
  {
    var json = await new StreamReader(Request.Body).ReadToEndAsync();

    try
    {
      var stripeEvent = ConstructStripeEvent(json);

      if (stripeEvent.Data.Object is not PaymentIntent intent)
      {
        return BadRequest("Invalid event data");
      }

      await HandlePaymentIntentSucceeded(intent);

      return Ok();
    }
    catch (StripeException ex)
    {
      logger.LogError(ex, "Stripe webhook error!");
      return StatusCode(StatusCodes.Status500InternalServerError, "Webhook error!");
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "An unexcepected error!");
      return StatusCode(StatusCodes.Status500InternalServerError, "An unexcepected error!");
    }
  }

  private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
  {
    if (intent.Status == "succeeded")
    {
      var spec = new OrderSpecification(intent.Id, true);
      var order = await unit.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpecificationAsync(spec, default) ??
      throw new Exception("Order not found!");

      if ((long)order.GetTotal() * 100 != intent.Amount)
      {
        order.Status = OrderStatus.PaymentMismatch;
      }
      else
      {
        order.Status = OrderStatus.PaymentReceived;
      }

      await unit.Compolete();
    }
  }



  private Event ConstructStripeEvent(string json)
  {
    try
    {
      return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);

    }
    catch (Exception ex)
    {
      logger.LogError(ex, "failed to Construct stripe event!");
      throw new StripeException("Invalid stripe signature!");
    }
  }
}