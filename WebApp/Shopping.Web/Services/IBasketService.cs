using Refit;
using Shopping.Web.Models.Basket;
using System.Net;

namespace Shopping.Web.Services
{
    public interface IBasketService
    {
        [Get("/basket-service/basket/{userName}")]
        Task<GetShoppingCartResponse> GetBasket(string userName);

        [Post("/basket-service/basket")]
        Task<StoreBasketResponse> StoreBasket(StoreBasketRequset request);

        [Delete("/basket-service/basket/{userName}")]
        Task<DeleteBasketResponse> DeleteBasket(string userName);

        [Post("/basket-service/basket/checkout")]
        Task<BasketCheckOutResponse> CheckoutBasket(BasketCheckOutRequest request);

        public async Task<ShippingCartModel> LoadUserBasket()
        {
            var userName = "osama";
            ShippingCartModel basket;

            try
            {
                var getBasketResponse = await GetBasket(userName);
                basket = getBasketResponse.Cart;
            }
            catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.NotFound)
            {
                basket = new ShippingCartModel
                {
                    UserId = userName,
                    Items = []
                };
            }

            return basket;
        }
    }
}