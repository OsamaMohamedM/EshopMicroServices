using Basket.API.Data.Models;
using Basket.API.Data.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data.Repositries
{
    public class CacheBasketRepository(IBasketRepository basketRepository, IDistributedCache cache) : IBasketRepository
    {
        public async Task<bool> DeleteShoppingCartAsync(string UserName, CancellationToken cancellation = default)
        {
            await basketRepository.DeleteShoppingCartAsync(UserName, cancellation);
            await cache.RemoveAsync(UserName);
            return true;
        }

        public async Task<ShoppingCart> GetShoppingCartAsync(string UserName, CancellationToken cancellation = default)
        {
            var cachedCart = await cache.GetStringAsync(UserName, cancellation);
            if (!string.IsNullOrEmpty(cachedCart))
                return JsonSerializer.Deserialize<ShoppingCart>(cachedCart);
            var baskets = await basketRepository.GetShoppingCartAsync(UserName, cancellation);
            await cache.SetStringAsync(UserName, JsonSerializer.Serialize(baskets), cancellation);
            return baskets;
        }

        public async Task<ShoppingCart> StoreShoppingCartAsync(ShoppingCart shoppingCart, CancellationToken cancellation = default)
        {
            await basketRepository.StoreShoppingCartAsync(shoppingCart, cancellation);
            await cache.SetStringAsync(shoppingCart.UserName, JsonSerializer.Serialize(shoppingCart), cancellation);
            return shoppingCart;
        }
    }
}