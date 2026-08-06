namespace SimplePos.Domain.Common.ResultPattern;
public sealed record Error
{
    public string Code { get; init; }
    public string Description { get; init; }
    public ErrorType Type { get; init; }
    public Error(string code, string description, ErrorType type = ErrorType.Validation)
    {
        Code = code;
        Description = description;
        Type = type;
    }
    public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.None);
}