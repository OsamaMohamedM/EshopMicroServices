using OrderingDomain.Abestrations;
using OrderingDomain.Models;

namespace OrderingDomain.Evenets
{
    public record OrderCreatedEvent(Order order) : IDomainEvent;
}