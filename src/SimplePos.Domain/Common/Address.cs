using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Common;
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

    public static Result<Address> Create(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<Address>.Failure(AddressError.StreetEmpty);
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<Address>.Failure(AddressError.CityEmpty);
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            return Result<Address>.Failure(AddressError.StateEmpty);
        }

        if (string.IsNullOrWhiteSpace(postalCode))
        {
            return Result<Address>.Failure(AddressError.PostalCodeEmpty);
        }

        if (string.IsNullOrWhiteSpace(country))
        {
            return Result<Address>.Failure(AddressError.CountryEmpty);
        }

        return Result<Address>.Success(new Address(street, city, state, postalCode, country));
    }
}
