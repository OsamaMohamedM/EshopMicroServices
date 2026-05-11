using Basket.API.Data.Models;

namespace Basket.API.Data.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetShoppingCartAsync(string UserName, CancellationToken cancellation = default);

        Task<ShoppingCart> StoreShoppingCartAsync(ShoppingCart shoppingCart, CancellationToken cancellation = default);

        Task<bool> DeleteShoppingCartAsync(string UserName, CancellationToken cancellation = default);
    }
}