using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Users;
public static class UserError
{
    public static readonly Error UsernameEmpty = new Error(
        "Users.UsernameEmpty", "Username cannot be empty.", ErrorType.Validation);
    public static readonly Error PasswordEmpty = new Error(
        "Users.PasswordEmpty", "Password cannot be empty.", ErrorType.Validation);
    public static readonly Error EmailEmpty = new Error(
        "Users.EmailEmpty", "Email cannot be empty.", ErrorType.Validation);
    public static readonly Error NewPasswordEmpty = new Error(
        "Users.NewPasswordEmpty", "New password cannot be empty.", ErrorType.Validation);
    public static readonly Error AlreadySoftDeleted = new Error(
        "Users.AlreadySoftDeleted", "User is already soft deleted.", ErrorType.Conflict);
    public static readonly Error SoftDeleted = new Error(
        "Users.SoftDeleted", "Operation cannot be performed on a soft deleted user.", ErrorType.Conflict);
    public static readonly Error PermissionAlreadyAssigned = new Error(
        "Users.PermissionAlreadyAssigned", "Permission is already assigned to this user.", ErrorType.Conflict);
    public static readonly Error PermissionNotAssigned = new Error(
        "Users.PermissionNotAssigned", "Permission is not assigned to this user.", ErrorType.NotFound);
    public static readonly Error PermissionIdEmpty = new Error(
        "Users.PermissionIdEmpty", "Permission ID cannot be empty.", ErrorType.Validation);
    public static readonly Error InvalidCredentials = new(
        "Users.InvalidCredentials", "Invalid email or password.", ErrorType.Unauthorized);
    public static readonly Error AccountLocked = new(
        "Users.AccountLocked", "User account is locked.", ErrorType.Forbidden);
    public static readonly Error AccountNotFound = new(
        "Users.AccountNotFound", "User account not found.", ErrorType.NotFound);
}

