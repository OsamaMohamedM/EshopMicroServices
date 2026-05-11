using Basket.API.Data.Models;
using Basket.API.Data.Repositories;
using BuildingBlocks.CQRS;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(bool Success, string UserName);

    public class StoreBasketHandler(IBasketRepository repository) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = await repository.StoreShoppingCartAsync(request.Cart);
            return new StoreBasketResult(basket != null, request.Cart.UserName);
        }
    }
}