namespace OrderingDomain.ValueObject
{
    public record Payment
    {
        public string CardName { get; init; } = default!;
        public string CardNumber { get; init; } = default!;
        public string Expiration { get; init; } = default!;
        public string Cvv { get; init; } = default!;
        public int PaymentMethod { get; init; }

        private Payment(
            string cardName,
            string cardNumber,
            string expiration,
            string cvv,
            int paymentMethod)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            Cvv = cvv;
            PaymentMethod = paymentMethod;
        }

        public static Payment Of(
            string cardName,
            string cardNumber,
            string expiration,
            string cvv,
            int paymentMethod)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardName);
            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(expiration);
            ArgumentException.ThrowIfNullOrWhiteSpace(cvv);

            return new Payment(
                cardName,
                cardNumber,
                expiration,
                cvv,
                paymentMethod);
        }
    }
}