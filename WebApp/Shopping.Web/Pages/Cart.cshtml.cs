using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Basket;
using Shopping.Web.Services;

namespace Shopping.Web.Pages
{
    public class CartModel(IBasketService basketService, ILogger<CartModel> logger) : PageModel
    {
        public ShippingCartModel Cart { get; set; } = new ShippingCartModel();

        public async Task<IActionResult> OnGet()
        {
            Cart = await basketService.LoadUserBasket();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveToCartAsync(Guid productId)
        {
            logger.LogInformation("Remove to cart button clicked");
            Cart = await basketService.LoadUserBasket();

            Cart.Items.RemoveAll(x => x.ProductId == productId);
            await basketService.StoreBasket(new StoreBasketRequset(Cart));
            return RedirectToPage();
        }
    }
}