using Basket.API.Data.Models;
using Basket.API.Data.Repositories;
using BuildingBlocks.CQRS;
using Discount.Grpc.Protos;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(bool Success, string UserName);

    // ToDoo Comunicate with DiscountService via gRPC to calculate the total price of the basket
    public class StoreBasketHandler(IBasketRepository repository,
        DiscountProtoService.DiscountProtoServiceClient discountProto) :
        ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
        {
            await DeducteDiscount(request.Cart, cancellationToken);
            var basket = await repository.StoreShoppingCartAsync(request.Cart);
            return new StoreBasketResult(basket != null, request.Cart.UserName);
        }

        private async Task DeducteDiscount(ShoppingCart shoppingCart, CancellationToken cancellation)
        {
            foreach (var item in shoppingCart.Items)
            {
                var discount = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName });
                item.Price -= discount.Coupon.Amount;
            }
        }
    }
}