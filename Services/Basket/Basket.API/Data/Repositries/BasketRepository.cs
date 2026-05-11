using Basket.API.Data.Models;
using Basket.API.Exceptions;
using Marten;

namespace Basket.API.Data.Repositories
{
    public class BasketRepository(IDocumentSession session) : IBasketRepository
    {
        public async Task<bool> DeleteShoppingCartAsync(string UserName, CancellationToken cancellation = default)
        {
            session.Delete<ShoppingCart>(UserName);
            await SaveChanges(cancellation);
            return true;
        }

        public async Task<ShoppingCart> GetShoppingCartAsync(string UserName, CancellationToken cancellation = default)
        {
            var basket = await session.LoadAsync<ShoppingCart>(UserName, cancellation);
            return basket is null ? throw new BasketNotFound(UserName) : basket;
        }

        private async Task SaveChanges(CancellationToken cancellation = default)
        {
            await session.SaveChangesAsync(cancellation);
        }

        public async Task<ShoppingCart> StoreShoppingCartAsync(ShoppingCart shoppingCart, CancellationToken cancellation = default)
        {
            session.Store(shoppingCart);
            await SaveChanges(cancellation);
            return shoppingCart;
        }
    }
}