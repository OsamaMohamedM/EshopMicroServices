using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Order;
using Shopping.Web.Services;

namespace Shopping.Web.Pages
{
    public class OrderResponseModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string OrderName { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string EmailAddress { get; set; } = default!;
        public string AddressLine { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
        public decimal TotalPrice { get; set; }
    }

    public class OrderListModel(IOrderingService orderingService, IBasketService basketService, ILogger<OrderListModel> logger)
        : PageModel
    {
        public IEnumerable<OrderResponseModel> Orders { get; set; } = Enumerable.Empty<OrderResponseModel>();

        public async Task<IActionResult> OnGet()
        {
            logger.LogInformation("Retrieving order history for current customer.");
            var customerId = new Guid("5812B601-987A-4B60-B147-3855E5186E97");
            var response = await orderingService.GetOrdersByCustomer(customerId);

            Orders = response.Orders.Select(o => new OrderResponseModel
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                OrderName = o.OrderName,
                FirstName = o.ShippingAddress?.FirstName ?? string.Empty,
                LastName = o.ShippingAddress?.LastName ?? string.Empty,
                EmailAddress = o.ShippingAddress?.EmailAddress ?? string.Empty,
                AddressLine = o.ShippingAddress?.AddressLine ?? string.Empty,
                Country = o.ShippingAddress?.Country ?? string.Empty,
                State = o.ShippingAddress?.State ?? string.Empty,
                ZipCode = o.ShippingAddress?.ZipCode ?? string.Empty,
                TotalPrice = o.OrderItems?.Sum(x => x.Price * x.Quantity) ?? 0
            });

            return Page();
        }
    }
}
