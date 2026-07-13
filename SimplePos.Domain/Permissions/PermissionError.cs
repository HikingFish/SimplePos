using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Permissions;
public static class PermissionError
{
    public static readonly Error PermissionNameEmpty = new Error(
        "Permissions.PermissionNameEmpty", "Permission name cannot be empty");
    public static readonly Error UserIdEmpty = new Error(
        "Permissions.UserIdEmpty", "User ID cannot be empty.");
    public static readonly Error PermissionIdEmpty = new Error(
        "Permissions.PermissionIdEmpty", "Permission ID cannot be empty.");
    public static readonly Error UserPermissionNotFound = new Error(
        "Permissions.UserPermissionNotFound", "User permission association not found.");
}

