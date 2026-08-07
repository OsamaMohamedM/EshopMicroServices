using MediatR;
using Microsoft.Extensions.Logging;
using OrderingDomain.Evenets;

namespace OrderingApplication.Orders.EventHandler.Domain
{
    public class OrderUpdatedEventHandler(ILogger<OrderUpdatedEventHandler> _logger) : INotificationHandler<OrderUpdatedEvent>
    {
        public Task Handle(OrderUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("OrderUpdatedEventHandler called for OrderId: {OrderId}", notification.order.Id);
            throw new NotImplementedException();
        }
    }
}