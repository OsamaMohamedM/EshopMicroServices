using MediatR;
using Microsoft.Extensions.Logging;
using OrderingDomain.Evenets;

namespace OrderingApplication.Orders.EventHandler.Domain
{
    public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> _logger) : INotificationHandler<OrderCreatedEvent>
    {
        public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderCreatedEventHandler called for OrderId: {OrderId}", notification.order.Id);
            throw new NotImplementedException();
        }
    }
}