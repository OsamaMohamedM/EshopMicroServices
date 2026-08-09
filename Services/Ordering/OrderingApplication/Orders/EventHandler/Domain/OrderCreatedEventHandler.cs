using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using OrderingApplication.Orders.Extensions;
using OrderingDomain.Evenets;

namespace OrderingApplication.Orders.EventHandler.Domain;

public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> _logger, IFeatureManager featureManager, IPublishEndpoint publishEndpoint) : INotificationHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OrderCreatedEventHandler called for OrderId: {OrderId}", notification.order.Id);
        if (await featureManager.IsEnabledAsync("EnableOrderingService"))
        {
            var orderCreatedIntegrationEvent = notification.order.ToOrderDto();
            await publishEndpoint.Publish(orderCreatedIntegrationEvent, cancellationToken);
            return;
        }
    }
}