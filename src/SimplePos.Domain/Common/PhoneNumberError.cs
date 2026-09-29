using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Common;

public static class PhoneNumberError
{
    public static readonly Error Empty = new Error(
        "PhoneNumber.Empty", "Phone number cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = new Error(
        "PhoneNumber.InvalidFormat", "Invalid phone number format.", ErrorType.Validation);
    public static readonly Error PhoneNumberNotFound = new Error(
        "PhoneNumber.PhoneNumberNotFound", "Phone number not found.", ErrorType.NotFound);
}
