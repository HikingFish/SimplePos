using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Permissions;
public static class PermissionError
{
    public static readonly Error PermissionNameEmpty = new Error(
        "Permissions.PermissionNameEmpty", "Permission name cannot be empty");
}

