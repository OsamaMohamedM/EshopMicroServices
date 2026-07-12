using OrderingDomain.Exceptions;

namespace OrderingDomain.ValueObject
{
    public record ProductId
    {
        public Guid Value { get; }
        private ProductId(Guid value)
        {
            Value = value;
        }
        public static ProductId Create(Guid value)
        {
            if (value == Guid.Empty)
                throw new OrderDomainException("Product ID cannot be empty.");
            return new ProductId(value);
        }
    }
}