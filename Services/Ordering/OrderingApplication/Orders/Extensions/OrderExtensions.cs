namespace OrderingApplication.Orders.Extensions;

public static class OrderExtensions
{
    public static IQueryable<OrderDto> ProjectToOrderDto(this IQueryable<Order> orders)
    {
        return orders.Select(order => new OrderDto(
            order.Id.Value,
            order.CustomerId.Value,
            order.OrderName.Value,
            new AddressDto(
                order.ShippingAddress.FirstName,
                order.ShippingAddress.LastName,
                order.ShippingAddress.AddressLine,
                order.ShippingAddress.EmailAddress ?? string.Empty,
                order.ShippingAddress.Country),
            new AddressDto(
                order.BillingAddress.FirstName,
                order.BillingAddress.LastName,
                order.BillingAddress.AddressLine,
                order.BillingAddress.EmailAddress ?? string.Empty,
                order.BillingAddress.Country),
            new PaymentDto(
                order.Payment.CardName,
                order.Payment.CardNumber,
                order.Payment.Expiration,
                order.Payment.Cvv,
                order.Payment.PaymentMethod),
            order.Status,
            order.Items.Select(item => new OrderItemDto(
                item.OrderId.Value,
                item.ProductId.Value,
                item.UnitPrice.Value,
                item.Quantity.Value)).ToList()));
    }
}
