using OrderingDomain.Exceptions;

namespace OrderingDomain.ValueObject
{
    public record Quantity
    {
        public int Value { get; private set; }
        private Quantity(int value)
        {
            Value = value;
        }
        public static Quantity Create(int value)
        {
            if (value <= 0)
                throw new OrderDomainException($"Quantity must be greater than zero. its {value}");
            return new Quantity(value);
        }
    }
}