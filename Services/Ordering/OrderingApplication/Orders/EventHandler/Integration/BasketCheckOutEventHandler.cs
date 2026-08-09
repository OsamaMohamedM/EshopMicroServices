using BuildingBlocksMessageing.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderingApplication.Orders.Commands.CreateOrder;
using OrderingDomain.Enums;

namespace OrderingApplication.Orders.EventHandler.Integration;

public class BasketCheckOutEventHandler(ISender sender, ILogger<BasketCheckOutEventHandler> logger) : IConsumer<BasketCheckOutEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckOutEvent> context)
    {
        logger.LogInformation("BasketCheckOutEventHandler: Consuming BasketCheckOutEvent with OrderId: {OrderId}", context.Message.Id);
        var command = MapToCreateOrderCommand(context.Message);
        await sender.Send(command, context.CancellationToken);
    }

    private CreatOrderCommand MapToCreateOrderCommand(BasketCheckOutEvent message)
    {
        var addressDto = new AddressDto(
            message.FirstName,
            message.LastName,
            message.AddressLine,
            message.EmailAddress,
            message.Country,
            message.State,
            message.ZipCode
        );

        var paymentDto = new PaymentDto(
            message.CardName,
            message.CardNumber,
            message.CardExpiration,
            message.CVV,
            message.PaymentMethod
        );

        var orderId = Guid.NewGuid();
        var OrderItem = message.BasketItems.Select(item => new OrderItemDto(
            OrderId: orderId,
            ProductId: item.ProductId,
            Price: item.Price,
            Quantity: item.Quantity
        )).ToList();
        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: message.CustomerId,
            OrderName: message.UserName,
            ShippingAddress: addressDto,
            BillingAddress: addressDto,
            Payment: paymentDto,
            Status: OrderStatus.Pending,
            Items: OrderItem
        );

        return new CreatOrderCommand(orderDto);
    }
}