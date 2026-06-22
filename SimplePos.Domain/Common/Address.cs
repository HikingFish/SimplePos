namespace SimplePos.Domain.Common
{
    public record Address
    {
        public string Street { get; init; }
        public string City { get; init; }
        public string State { get; init; }
        public string PostalCode { get; init; }
        public string Country { get; init; }

        private Address(string street, string city, string state, string postalCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        public static Address Create(string street, string city, string state, string postalCode, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
            {
                throw new DomainException("Street cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                throw new DomainException("City cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                throw new DomainException("State cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(postalCode))
            {
                throw new DomainException("Postal code cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(country))
            {
                throw new DomainException("Country cannot be empty.");
            }

            return new Address(street, city, state, postalCode, country);
        }
    }
}