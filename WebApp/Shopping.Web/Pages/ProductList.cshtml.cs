using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Basket;
using Shopping.Web.Models.Catalog;
using Shopping.Web.Services;

namespace Shopping.Web.Pages
{
    public class ProductListModel(ICatalogService catalogService, IBasketService basketService, ILogger<ProductListModel> logger)
        : PageModel
    {
        public IEnumerable<ProductModel> ProductsList { get; private set; } = Enumerable.Empty<ProductModel>();
        public IEnumerable<string> CategoryList { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; private set; } = default!;

        public async Task<IActionResult> OnGet(string Category)
        {
            if (!String.IsNullOrEmpty(Category))
            {
                var response = await catalogService.GetProductsByCategory(Category);
                ProductsList = response.Products;
                SelectedCategory = Category;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddToCart(Guid productId)
        {
            logger.LogInformation("Adding product with ID {ProductId} to cart.", productId);
            var product = await catalogService.GetProductById(productId);
            if (product is null)
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
                Quantity = 1
            });
            await basketService.StoreBasket(new StoreBasketRequset(basket));
            return RedirectToPage("/Cart");
        }
    }
}