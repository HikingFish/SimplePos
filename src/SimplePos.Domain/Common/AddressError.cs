using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Common;

public static class AddressError
{
    public static readonly Error StreetEmpty = new Error(
        "Address.StreetEmpty", "Street cannot be empty.", ErrorType.Validation);
    public static readonly Error CityEmpty = new Error(
        "Address.CityEmpty", "City cannot be empty.", ErrorType.Validation);
    public static readonly Error StateEmpty = new Error(
        "Address.StateEmpty", "State cannot be empty.", ErrorType.Validation);
    public static readonly Error PostalCodeEmpty = new Error(
        "Address.PostalCodeEmpty", "Postal code cannot be empty.", ErrorType.Validation);
    public static readonly Error CountryEmpty = new Error(
        "Address.CountryEmpty", "Country cannot be empty.", ErrorType.Validation);
    public static readonly Error AddressNotFound = new Error(
        "Address.AddressNotFound", "Address not found.", ErrorType.NotFound);
}
