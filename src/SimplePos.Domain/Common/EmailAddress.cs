using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Common;
public record EmailAddress
{
    public string Value { get; init; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static Result<EmailAddress> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<EmailAddress>.Failure(EmailAddressError.Empty);
        }

        var emailAddress = new EmailAddress(email);

        if (!emailAddress.IsValidEmail(email))
        {
            return Result<EmailAddress>.Failure(EmailAddressError.InvalidFormat);
        }

        return Result<EmailAddress>.Success(emailAddress);
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}