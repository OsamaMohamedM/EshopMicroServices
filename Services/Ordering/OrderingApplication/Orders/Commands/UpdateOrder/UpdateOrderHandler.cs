using BuildingBlocks.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace OrderingApplication.Orders.Commands.UpdateOrder;

public class UpdateOrderHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
{
    public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Create(command.OrderDto.Id);
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order is null)
        {
            throw new NotFoundException($"Order with Id {command.OrderDto.Id} not found");
        }

        UpdateOrder(order, command.OrderDto);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateOrderResult(true);
    }

    private void UpdateOrder(Order order, OrderDto orderDto)
    {
        var shippingAddress = Address.Create(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var billingAddress = Address.Create(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);
        var payment = Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod);

        order.Update(
            OrderId.Create(orderDto.Id),
            CustomerId.Create(orderDto.CustomerId),
            OrderName.Create(orderDto.OrderName),
            shippingAddress,
            billingAddress,
            payment);

        foreach (var item in order.Items.ToList())
        {
            order.Remove(item.ProductId);
        }

        foreach (var item in orderDto.Items)
        {
            order.Add(ProductId.Create(item.ProductId), item.Quantity, item.Price);
        }
    }
}