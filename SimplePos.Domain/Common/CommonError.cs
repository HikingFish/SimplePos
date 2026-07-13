using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Common;

public static class CommonError
{
    public static class Address
    {
        public static readonly Error StreetEmpty = new Error(
            "Address.StreetEmpty", "Street cannot be empty.");
        public static readonly Error CityEmpty = new Error(
            "Address.CityEmpty", "City cannot be empty.");
        public static readonly Error StateEmpty = new Error(
            "Address.StateEmpty", "State cannot be empty.");
        public static readonly Error PostalCodeEmpty = new Error(
            "Address.PostalCodeEmpty", "Postal code cannot be empty.");
        public static readonly Error CountryEmpty = new Error(
            "Address.CountryEmpty", "Country cannot be empty.");
    }

    public static class EmailAddress
    {
        public static readonly Error Empty = new Error(
            "EmailAddress.Empty", "Email address cannot be empty.");
        public static readonly Error InvalidFormat = new Error(
            "EmailAddress.InvalidFormat", "Invalid email address format.");
    }
}
