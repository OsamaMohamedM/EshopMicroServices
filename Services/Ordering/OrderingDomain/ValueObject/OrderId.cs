using OrderingDomain.Exceptions;

namespace OrderingDomain.ValueObject
{
    public record OrderId
    {
        public Guid Value { get; }
        private OrderId(Guid value)
        {
            Value = value;
        }
        public static OrderId Create(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new OrderDomainException("OrderId cannot be empty.");
            }
            return new OrderId(value);
        }
    }
}