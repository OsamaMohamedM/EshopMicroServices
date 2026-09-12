namespace Shopping.Web.Models.Basket
{
    public class ShippingCartModel
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = default!;
        public List<ShoppingCartItemModel> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(item => item.Price * item.Quantity);
    }

    public class ShoppingCartItemModel
    {
        public string Color { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public record GetShoppingCartResponse(ShippingCartModel Cart);
    public record StoreBasketRequset(ShoppingCartItemModel Item);
    public record StoreBasketResponse(string UserName);
    public record DeleteBasketResponse(bool IsSuccess);
}