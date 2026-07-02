namespace SimplePos.Domain.Common;
public record EmailAddress
{
    public string Value { get; init; }

    private EmailAddress(string email)
    {
        Value = email;
    }

    public static EmailAddress Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email address cannot be empty.");
        }

        var emailAddress = new EmailAddress(email);

        if (!emailAddress.IsValidEmail(email))
        {
            throw new DomainException("Invalid email address format.");
        }

        return emailAddress;
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