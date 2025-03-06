using API.DTOs;
using API.Extentions;
using Core.Entities.OrderAggregate;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unit) : BaseApiController
{
    [HttpPost]
   public async Task<IActionResult> CreateOrder(CreateOrderDto orderDto)
{
    // Validate inputs
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var email = User.GetEmail();
    
    // Get and validate cart
    var cart = await cartService.GetCartAsync(orderDto.CartId);
    if (cart == null)
        return BadRequest("Cart not found");
    
    if (string.IsNullOrEmpty(cart.PaymentIntentId))
        return BadRequest("No payment intent for this order");

    // Get and validate delivery method
    var deliveryMethod = await GetDeliveryMethod(orderDto.DeliveryMethodId);
    if (deliveryMethod == null)
        return BadRequest("No delivery method selected");

    // Create order items
    var orderItemsResult = await CreateOrderItems(cart.Items);
    if (!orderItemsResult.IsSuccess)
        return BadRequest(orderItemsResult.Error);

    // Create and save order
    var order = new Core.Entities.OrderAggregate.Order
    {
        OrderItems = orderItemsResult.Items,
        DeliveryMethod = deliveryMethod,
        ShippingAddress = orderDto.ShippingAddress,
        Subtotal = CalculateSubtotal(orderItemsResult.Items),
        PaymentSummary = orderDto.PaymentSummary,
        PaymentIntentId = cart.PaymentIntentId,
        BuyerEmail = email
    };

    var result = await SaveOrder(order);
    return result.IsSuccess 
        ? Ok(order) 
        : BadRequest("Problem creating order");
}

private async Task<DeliveryMethod> GetDeliveryMethod(int deliveryMethodId)
{
    return await unit.Repository<DeliveryMethod>()
        .GetByIdAsync(deliveryMethodId, CancellationToken.None);
}

private async Task<(bool IsSuccess, List<OrderItem> Items, string Error)> CreateOrderItems(IEnumerable<CartItem> cartItems)
{
    var orderItems = new List<OrderItem>();

    foreach (var item in cartItems)
    {
        var productItem = await unit.Repository<Product>()
            .GetByIdAsync(item.ProductId, CancellationToken.None);

        if (productItem == null)
            return (false, null, $"Product not found: {item.ProductId}");

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

        orderItems.Add(orderItem);
    }

    return (true, orderItems, null);
}

private decimal CalculateSubtotal(IEnumerable<OrderItem> items)
{
    return items.Sum(x => x.Price * x.Quantity);
}

private async Task<(bool IsSuccess, string Error)> SaveOrder(Core.Entities.OrderAggregate.Order order)
{
    await unit.Repository<Core.Entities.OrderAggregate.Order>()
        .AddAsync(order, default);

    return await unit.Compolete() 
        ? (true, null) 
        : (false, "Failed to save order");
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

