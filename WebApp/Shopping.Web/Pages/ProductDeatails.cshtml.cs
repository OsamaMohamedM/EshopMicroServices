using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Basket;
using Shopping.Web.Models.Catalog;
using Shopping.Web.Services;

namespace Shopping.Web.Pages
{
    public class ProductDetailModel(ICatalogService catalogService, IBasketService basketService, ILogger<ProductDetailModel> logger)
        : PageModel
    {
        public ProductModel Product { get; set; } = default!;

        [BindProperty]
        public string Color { get; set; } = default!;

        [BindProperty]
        public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnGet(Guid productId)
        {
            logger.LogInformation("Retrieving details for product with ID {ProductId}.", productId);
            var response = await catalogService.GetProductById(productId);
            if (response?.Product is null)
            {
                logger.LogWarning("Product with ID {ProductId} not found.", productId);
                return NotFound();
            }

            Product = response.Product;
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCart(Guid productId, int quantity, string color)
        {
            logger.LogInformation("Adding product with ID {ProductId} to cart.", productId);
            var product = await catalogService.GetProductById(productId);
            if (product?.Product is null)
            {
                logger.LogWarning("Product with ID {ProductId} not found.", productId);
                return NotFound();
            }

            var basket = await basketService.LoadUserBasket();
            basket.Items.Add(new ShoppingCartItemModel
            {
                ProductId = product.Product.Id,
                ProductName = product.Product.Name,
                Price = product.Product.Price,
                Quantity = quantity > 0 ? quantity : 1,
                Color = color
            });

            await basketService.StoreBasket(new StoreBasketRequset(basket));
            return RedirectToPage("/Cart");
        }
    }

    public class ProductDeatailsModel(ICatalogService catalogService, IBasketService basketService, ILogger<ProductDetailModel> logger)
        : ProductDetailModel(catalogService, basketService, logger)
    {
    }
}
