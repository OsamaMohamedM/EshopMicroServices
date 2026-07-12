namespace OrderingDomain.ValueObject
{
    public record Money
    {
        public decimal Value;
        public static Money Of(decimal value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value, nameof(value));
            return new Money { Value = value };
        }
    }
}