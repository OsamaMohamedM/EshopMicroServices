using MediatR;

namespace OrderingDomain.Abestrations
{
    public interface IDomainEvent : INotification
    {
        public Guid EvenId => Guid.NewGuid();
        public DateTime OccurredOn => DateTime.UtcNow;
        public string EventType => GetType().AssemblyQualifiedName;
    }
}