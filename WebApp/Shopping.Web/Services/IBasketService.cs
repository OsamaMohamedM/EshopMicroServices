using Refit;
using Shopping.Web.Models.Basket;

namespace Shopping.Web.Services
{
    public interface IBasketService
    {
        [Get("/basket-service/basket/{userName}")]
        Task<GetShoppingCartResponse> GetBasket(string userName);

        [Post("/basket-service/basket")]
        Task<StoreBasketResponse> StoreBasket(BasketCheckOutRequest request);

        [Delete("/basket-service/basket/{userName}")]
        Task<DeleteBasketResponse> DeleteBasket(string userName);

        [Post("/basket-service/basket/checkout")]
        Task<BasketCheckOutResponse> CheckoutBasket(BasketCheckOutRequest request);
    }
}