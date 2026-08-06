using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Common;

public static class EmailAddressError
{
    public static readonly Error Empty = new Error(
        "EmailAddress.Empty", "Email address cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidFormat = new Error(
        "EmailAddress.InvalidFormat", "Invalid email address format.", ErrorType.Validation);
    public static readonly Error EmailAddressNotFound = new Error(
        "EmailAddress.EmailAddressNotFound", "Email address not found.", ErrorType.NotFound);
}