using OrderingDomain.Exceptions;

namespace OrderingDomain.ValueObject
{
    public record OrderName
    {
        public string Value { get; }
        private OrderName(string value)
        {
            Value = value;
        }
        public static OrderName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new OrderDomainException("Order name cannot be empty.");
            if (value.Length > 100)
                throw new OrderDomainException("Order name cannot exceed 100 characters.");
            return new OrderName(value);
        }
    }
}