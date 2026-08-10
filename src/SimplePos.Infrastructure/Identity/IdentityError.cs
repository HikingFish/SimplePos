using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Infrastructure.Identity;

public class IdentityError
{
    public static Error OutletNotFound = new Error("IdentityError.OutletNotFound", "Outlet does not exist", ErrorType.NotFound);
    public static Error CompanyNotFound = new Error("IdentityError.CompanyNotFound", "Company does not exist", ErrorType.NotFound);
}