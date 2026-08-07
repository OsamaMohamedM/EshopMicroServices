using BuildingBlocksMessageing.Events;
using MassTransit;

namespace OrderingApplication.Orders.EventHandler.Integration;

public class BasketCheckOutEventHandler : IConsumer<BasketCheckOutEvent>
{
    public Task Consume(ConsumeContext<BasketCheckOutEvent> context)
    {
        throw new NotImplementedException();
    }
}