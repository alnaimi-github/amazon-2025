namespace Core.Intarfaces;

public interface IPaymentService
{
    Task<ShoppingCart> CreateOrUpdatePaymentIntent(string cartId);
    
}