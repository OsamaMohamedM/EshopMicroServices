namespace OrderingDomain.ValueObject
{
    public record Address
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string? EmailAddress { get; init; }
        public string AddressLine { get; init; }
        public string Country { get; init; }

        private Address(
            string firstName,
            string lastName,
            string? emailAddress,
            string addressLine,
            string country
            )
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            AddressLine = addressLine;
            Country = country;
        }

        public static Address Create(
            string firstName,
            string lastName,
            string? emailAddress,
            string addressLine,
            string country
            )
        {
            return new Address(
                firstName,
                lastName,
                emailAddress,
                addressLine,
                country
                );
        }
    }
}