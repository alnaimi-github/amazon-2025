using API.DTOs;
using API.Extentions;
using Core.Entities.OrderAggregate;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[Authorize]
public class OrderController(ICartService cartService, IUnitOfWork unit) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto orderDto)
    {

        var email = User.GetEmail();

        var cart = await cartService.GetCartAsync(orderDto.CartId);

        if (cart == null) return BadRequest("Cart not found");

        if (cart.PaymentIntentId == null) return BadRequest("No payment intent for this order");

        var items = new List<OrderItem>();

        foreach (var item in cart.Items)
        {

            var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId,
            cancellationToken: CancellationToken.None);

            if (productItem == null) return BadRequest("Problem with the order");

            var itemOrdered = new ProductItemOrdered
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PectureUrl = item.PictureUrl
            };
            var orderItem = new OrderItem
            {

                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity
            };

            items.Add(orderItem);
        }
        var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId,
                                   CancellationToken.None);
        if (deliveryMethod == null) return BadRequest("No delivery method selected");

        var order = new Core.Entities.OrderAggregate.Order
        {

            OrderItems = items,
            DeliveryMethod = deliveryMethod,
            ShippingAddress = orderDto.ShippingAddress,
            Subtotal = items.Sum(x => x.Price * x.Quantity),
            PaymentSummary = orderDto.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            BuyerEmail = email

        };
        await unit.Repository<Core.Entities.OrderAggregate.Order>().AddAsync(order, CancellationToken.None);

        if (await unit.Compolete())
        {
            return Ok(order);

        }
        return BadRequest("Problem creating order");
    }
    [HttpGet]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var spec = new OrderSpecification(User.GetEmail());
        var orders = await unit.Repository<Core.Entities.OrderAggregate.Order>().ListSpecificationAsync(spec, CancellationToken.None);
        var ordersToReturn = orders.Select(o => o.ToDto()).ToList();
        return Ok(ordersToReturn);
    }


    [HttpGet("{id:int}")]

    public async Task<IActionResult> GetOrderById(int id)
    {

        var spec = new OrderSpecification(User.GetEmail(), id);

        var order = await unit.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpecificationAsync(spec, CancellationToken.None);

        if (order == null) return NotFound();

        return Ok(order.ToDto());
    }

}

