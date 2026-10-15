using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Basket;
using Shopping.Web.Models.Catalog;
using Shopping.Web.Services;

namespace Shopping.Web.Pages
{
    public class IndexModel(ILogger<IndexModel> logger, ICatalogService catalogService, IBasketService basketService) : PageModel
    {
        public IEnumerable<ProductModel> ProductList { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            logger.LogInformation("Index page requested");
            var result = await catalogService.GetProducts(2, 3);
            ProductList = result.Products;
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
        {
            logger.LogInformation("Add to cart button clicked");

            var productResponse = await catalogService.GetProductById(productId);

            var basket = await LoadUserBasket(basketService);

            basket.Items.Add(new ShoppingCartItemModel
            {
                ProductId = productId,
                ProductName = productResponse.Product.Name,
                Price = productResponse.Product.Price,
                Quantity = 1,
                Color = "Black"
            });

            await basketService.StoreBasket(new StoreBasketRequset(basket));

            return RedirectToPage("Cart");
        }

        private static async Task<ShippingCartModel> LoadUserBasket(IBasketService basketService)
        {
            var userName = "osama";
            ShippingCartModel basket;

            try
            {
                var getBasketResponse = await basketService.GetBasket(userName);
                basket = getBasketResponse.Cart;
            }
            catch (Exception)
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