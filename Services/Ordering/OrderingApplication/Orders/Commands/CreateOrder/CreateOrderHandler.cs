namespace OrderingApplication.Orders.Commands.CreateOrder;

public class CreateOrderHandler(IApplicationDbContext dbContext) : ICommandHandler<CreatOrderCommand, CreatOrderResult>
{
    public async Task<CreatOrderResult> Handle(CreatOrderCommand command, CancellationToken cancellationToken)
    {
        var order = CreateOrder(command.OrderDto);
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreatOrderResult(order.Id.Value);
    }

    private Order CreateOrder(OrderDto orderDto)
    {
        var shippingAddress = Address.Create(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var BillingAddress = Address.Create(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);
        var payment = Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod);

        var order = Order.Create(
            OrderId.Create(Guid.NewGuid()),
            CustomerId.Create(orderDto.CustomerId),
            OrderName.Create(orderDto.OrderName),
            shippingAddress,
            BillingAddress,
            payment);
        return order;
    }
}