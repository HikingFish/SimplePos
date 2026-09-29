using System.Text.RegularExpressions;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Common;

public record PhoneNumber
{
    private static readonly Regex PhoneRegex = new(@"^\+?[0-9\s\-\(\)\.]{3,20}$", RegexOptions.Compiled);

    public string Value { get; init; }

    private PhoneNumber()
    {
        Value = null!;
    }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static Result<PhoneNumber> Create(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result<PhoneNumber>.Failure(PhoneNumberError.Empty);
        }

        var trimmed = phoneNumber.Trim();

        if (!IsValidPhoneNumber(trimmed))
        {
            return Result<PhoneNumber>.Failure(PhoneNumberError.InvalidFormat);
        }

        return Result<PhoneNumber>.Success(new PhoneNumber(trimmed));
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (!PhoneRegex.IsMatch(phoneNumber))
        {
            return false;
        }

        var digitCount = phoneNumber.Count(char.IsDigit);
        return digitCount >= 3;
    }

    public override string ToString() => Value;
}
