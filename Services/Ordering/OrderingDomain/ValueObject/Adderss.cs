namespace OrderingDomain.ValueObject
{
    public record Address
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string? EmailAddress { get; init; }
        public string AddressLine { get; init; }
        public string Country { get; init; }
        public string State { get; init; }
        public string ZipCode { get; init; }

        private Address(
            string firstName,
            string lastName,
            string? emailAddress,
            string addressLine,
            string country,
            string state,
            string zipCode
            )
        {
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            AddressLine = addressLine;
            Country = country;
            State = state;
            ZipCode = zipCode;
        }

        public static Address Create(
            string firstName,
            string lastName,
            string? emailAddress,
            string addressLine,
            string country,
            string state,
            string zipCode
            )
        {
            return new Address(
                firstName,
                lastName,
                emailAddress,
                addressLine,
                country,
                state,
                zipCode
                );
        }
    }
}