using OrderingDomain.Abestrations;
using OrderingDomain.Models;

namespace OrderingDomain.Evenets
{
    public record OrderUpdatedEvent(Order order) : IDomainEvent;
}