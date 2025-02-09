using Stripe;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Services;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class PaymentService(
        IConfiguration config,
        ICartService cartService,
        IGenericRepository<Core.Entities.Product> productRepo,
        IGenericRepository<DeliveryMethod> dmRepo,
        ILogger<PaymentService> logger) : IPaymentService
    {
        private readonly IConfiguration _config = config;
        private readonly ICartService _cartService = cartService;
        private readonly IGenericRepository<Core.Entities.Product> _productRepo = productRepo;
        private readonly IGenericRepository<DeliveryMethod> _dmRepo = dmRepo;
        private readonly ILogger<PaymentService> _logger = logger;

        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId, CancellationToken cancellationToken)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
            var cart = await _cartService.GetCartAsync(cartId);

            if (cart is null) return null;

            var shippingPrice = 0m;

            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _dmRepo.GetByIdAsync((int)cart.DeliveryMethodId, cancellationToken);
                if (deliveryMethod is null) return null;

                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in cart.Items)
            {
                var productItem = await _productRepo.GetByIdAsync(item.ProductId, cancellationToken);
                if (productItem is null) return null;

                if (productItem.Price != item.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();
            PaymentIntent? intent = null;
            try
            {
                if (string.IsNullOrEmpty(cart.PaymentIntentId))
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100,
                        Currency = "usd",
                        PaymentMethodTypes = new List<string> { "card" }
                    };
                    intent = await service.CreateAsync(options, cancellationToken: cancellationToken);
                    cart.PaymentIntentId = intent.Id;
                    cart.ClientSecret = intent.ClientSecret;
                }
                else
                {
                    var options = new PaymentIntentUpdateOptions
                    {
                        Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100
                    };
                    intent = await service.UpdateAsync(cart.PaymentIntentId, options, cancellationToken: cancellationToken);
                }

                await _cartService.SetCartAsync(cart);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error occurred while creating/updating payment intent.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating/updating payment intent.");
                return null;
            }

            return cart;
        }
    }
}