using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Companies;
public static class CompanyError
{
    public static readonly Error CompanyNameEmpty = new Error(
        "Companies.CompanyNameEmpty", "Company name cannot be empty", ErrorType.Validation);
    public static readonly Error CompanyAddressNull = new Error(
        "Companies.CompanyAddressNull", "Company address cannot be null", ErrorType.Validation);
    public static readonly Error CompanyEmailNull = new Error(
        "Companies.CompanyEmailNull", "Company email cannot be null", ErrorType.Validation);
    public static readonly Error AlreadyActive = new Error(
        "Companies.AlreadyActive", "Company is already active", ErrorType.Conflict);
    public static readonly Error AlreadyInactive = new Error(
        "Companies.AlreadyInactive", "Company is already inactive", ErrorType.Conflict);
    public static readonly Error SoftDeleted = new Error(
        "Companies.SoftDeleted", "Operation cannot be performed on a soft deleted company", ErrorType.Conflict);
    public static readonly Error CompanyNotFound = new Error(
        "Companies.CompanyNotFound", "Company not found", ErrorType.NotFound);
    public static readonly Error CompanyAlreadyExists = new Error(
        "Companies.CompanyAlreadyExists", "Company already exists", ErrorType.Conflict);
}