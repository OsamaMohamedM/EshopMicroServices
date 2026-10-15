using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Basket;
using Shopping.Web.Services;
using BasketCheckoutModel = Shopping.Web.Models.Basket.BasketCheckOutModel;

namespace Shopping.Web.Pages
{
    public class CheckoutModel(IBasketService basketService, ILogger<CheckoutModel> logger)
        : PageModel
    {
        [BindProperty]
        public BasketCheckoutModel Order { get; set; } = default!;

        public ShippingCartModel Cart { get; set; } = default!;

        public async Task<IActionResult> OnGet()
        {
            Cart = await basketService.LoadUserBasket();
            return Page();
        }

        public async Task<IActionResult> OnPostCheckOut()
        {
            logger.LogInformation("Processing checkout for order.");
            Cart = await basketService.LoadUserBasket();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Order.CustomerId = new Guid("5812B601-987A-4B60-B147-3855E5186E97");
            Order.UserName = Cart.UserId;
            Order.TotalPrice = Cart.TotalPrice;

            await basketService.CheckoutBasket(new BasketCheckOutRequest(Order));
            return RedirectToPage("/Confirmation");
        }
    }

    public class CheckOutModel(IBasketService basketService, ILogger<CheckoutModel> logger)
        : CheckoutModel(basketService, logger)
    {
    }
}
