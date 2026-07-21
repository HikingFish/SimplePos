using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Users;
public static class UserError
{
    public static readonly Error UsernameEmpty = new Error(
        "Users.UsernameEmpty", "Username cannot be empty.");
    public static readonly Error PasswordEmpty = new Error(
        "Users.PasswordEmpty", "Password cannot be empty.");
    public static readonly Error EmailEmpty = new Error(
        "Users.EmailEmpty", "Email cannot be empty.");
    public static readonly Error NewPasswordEmpty = new Error(
        "Users.NewPasswordEmpty", "New password cannot be empty.");
    public static readonly Error AlreadySoftDeleted = new Error(
        "Users.AlreadySoftDeleted", "User is already soft deleted.");
    public static readonly Error SoftDeleted = new Error(
        "Users.SoftDeleted", "Operation cannot be performed on a soft deleted user.");
    public static readonly Error PermissionAlreadyAssigned = new Error(
        "Users.PermissionAlreadyAssigned", "Permission is already assigned to this user.");
    public static readonly Error PermissionNotAssigned = new Error(
        "Users.PermissionNotAssigned", "Permission is not assigned to this user.");
    public static readonly Error PermissionIdEmpty = new Error(
        "Users.PermissionIdEmpty", "Permission ID cannot be empty.");
    public static readonly Error InvalidCredentials = new(
        "Users.InvalidCredentials", "Invalid email or password.");
    public static readonly Error AccountLocked = new(
        "Users.AccountLocked", "User account is locked.");
}

