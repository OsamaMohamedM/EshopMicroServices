using OrderingDomain.Exceptions;

namespace OrderingDomain.ValueObject
{
    public record OrderItemId
    {
        public Guid Value { get; }
        private OrderItemId(Guid value)
        {
            Value = value;
        }
        public static OrderItemId Create(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new OrderDomainException("OrderItemId cannot be empty.");
            }
            return new OrderItemId(value);
        }
    }
}