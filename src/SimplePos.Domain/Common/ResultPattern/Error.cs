namespace SimplePos.Domain.Common.ResultPattern;
public sealed record Error
{
    public string Code { get; init; }
    public string Description { get; init; }
    public Error(string code, string description)
    {
        Code = code;
        Description = description;
    }
    public static readonly Error None = new Error(string.Empty, string.Empty);
}