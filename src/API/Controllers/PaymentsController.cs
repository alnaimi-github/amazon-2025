using API.Extentions;
using API.SignalR;
using Core.Entities.OrderAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers;

public class PaymentsController(
 IPaymentService paymentService,
 IUnitOfWork unit,
 IConfiguration config,
 IHubContext<NotificationHub> hubContext,
 ILogger<PaymentsController> logger)
 : BaseApiController
{

  private readonly string _whSecret = config["StripeSettings:WhSecret"]!;

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
        Core.Entities.OrderAggregate.Order order = null;
        int maxRetries = 5;
        int retryDelayMs = 1000;

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                var spec = new OrderSpecification(intent.Id,true);
                order = await unit.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpecificationAsync(spec,default);

                if (order != null)
                    break;

                logger.LogWarning($"Order not found. Retry attempt {i + 1}/{maxRetries}");
                await Task.Delay(retryDelayMs);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error fetching order during retry {i + 1}");
            }
        }

        if (order == null)
        {
            logger.LogError($"Order not found after {maxRetries} attempts. PaymentIntent: {intent.Id}");
            throw new Exception($"Order not found for PaymentIntent: {intent.Id}");
        }

        var orderTotal = (long)(order.GetTotal() * 100);
        if (orderTotal != intent.Amount)
        {
            order.Status = OrderStatus.PaymentMismatch;
            logger.LogWarning($"Amount mismatch. Order: {orderTotal}, Stripe: {intent.Amount}");
        }
        else
        {
            order.Status = OrderStatus.PaymentReceived;
            logger.LogInformation($"Payment received for order {order.Id}");
        }

      
        await unit.Compolete();

      
        var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
        if (!string.IsNullOrEmpty(connectionId))
        {
            await hubContext.Clients.Client(connectionId)
                .SendAsync("OrderCompleteNotification", order.ToDto());
            logger.LogInformation($"Notification sent to {order.BuyerEmail}");
        }
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