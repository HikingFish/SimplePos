using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Users;
public class User : ISoftDeletable
{
    public Guid UserId { get; private set; }
    public Guid OutletId { get; private set;}
    public string Username { get; private set; } 
    public string Password { get; private set; } 
    public EmailAddress Email { get; private set; } 
    public string? PhoneNumber { get; private set; } 
    public string? UserPosition { get; private set; } 
    public bool IsActive { get; private set; }
    public DateTime? DateTimeLastLogin { get; private set; }
    public DateTime DateTimeCreated { get; private set; }
    public bool SoftDeleted { get; private set; }
    public DateTime? DateTimeSoftDeleted { get; private set; }
    private readonly List<UserPermission> _userPermissions = new();
    public IReadOnlyCollection<UserPermission> UserPermissions => _userPermissions.AsReadOnly();
    private User() { }
    
    private User(Guid userId, Guid OutletId, string Username, string Password, EmailAddress Email, string PhoneNumber, string UserPosition)
    {
        UserId = userId;
        this.OutletId = OutletId;
        this.Username = Username;
        this.Password = Password;
        this.Email = Email;
        this.PhoneNumber = PhoneNumber;
        this.UserPosition = UserPosition;
        IsActive = true;
        DateTimeLastLogin = null;
        DateTimeCreated = DateTime.UtcNow;
        SoftDeleted = false;
        DateTimeSoftDeleted = null;
    }

    public static Result<User> Create(Guid OutletId, string Username, string Password, EmailAddress Email, string PhoneNumber, string UserPosition)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return Result<User>.Failure(UserError.UsernameEmpty);
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            return Result<User>.Failure(UserError.PasswordEmpty);
        }

        if (Email == null)
        {
            return Result<User>.Failure(UserError.EmailEmpty);
        }

        return Result<User>.Success(new User(Guid.CreateVersion7(), OutletId, Username, Password, Email, PhoneNumber, UserPosition));
    }

    public Result UpdateLastLogin()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        DateTimeLastLogin = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateUserInfo(EmailAddress newEmail, string newPhoneNumber, string newUserPosition, bool newIsActive)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (newEmail == null)
        {
            return Result.Failure(UserError.EmailEmpty);
        }

        Email = newEmail;
        PhoneNumber = newPhoneNumber;
        UserPosition = newUserPosition;
        IsActive = newIsActive;
        return Result.Success();
    }
    public Result UpdatePassword(string newHashedPassword)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (string.IsNullOrWhiteSpace(newHashedPassword))
        {
            return Result.Failure(UserError.NewPasswordEmpty);
        }

        Password = newHashedPassword;
        return Result.Success();
    }

    public Result SoftDelete()
    {
        if (SoftDeleted)
        {
            return Result.Failure(UserError.AlreadySoftDeleted);
        }

        SoftDeleted = true;
        DateTimeSoftDeleted = DateTime.UtcNow;
        IsActive = false;
        return Result.Success();
    }

    public Result AssignPermission(Guid permissionId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (permissionId == Guid.Empty)
        {
            return Result.Failure(UserError.PermissionIdEmpty);
        }

        if (_userPermissions.Any(up => up.PermissionId == permissionId))
        {
            return Result.Failure(UserError.PermissionAlreadyAssigned);
        }

        var userPermissionResult = UserPermission.Create(UserId, permissionId);
        if (!userPermissionResult.IsSuccess)
        {
            return Result.Failure(userPermissionResult.Error);
        }

        _userPermissions.Add(userPermissionResult.Data!);
        return Result.Success();
    }

    public Result RevokePermission(Guid permissionId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var userPermission = _userPermissions.FirstOrDefault(up => up.PermissionId == permissionId);
        if (userPermission == null)
        {
            return Result.Failure(UserError.PermissionNotAssigned);
        }

        _userPermissions.Remove(userPermission);
        return Result.Success();
    }

    private Result EnsureNotSoftDeleted()
    {
        if (SoftDeleted)
        {
            return Result.Failure(UserError.SoftDeleted);
        }
        return Result.Success();
    }
}
