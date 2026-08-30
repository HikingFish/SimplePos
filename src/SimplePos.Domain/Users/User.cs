using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Users;
public class User : ISoftDeletable
{
    public Guid UserId { get; private set; }
    public Guid? OutletId { get; private set;}
    public Guid CompanyId { get; private set; }
    public string Username { get; private set; }
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
    private readonly List<UserOutletAccess> _userOutletAccesses = new();
    public IReadOnlyCollection<UserOutletAccess> UserOutletAccesses => _userOutletAccesses.AsReadOnly();
    private User() { }
    
    private User(Guid userId,Guid CompanyId, Guid? OutletId, string Username, EmailAddress Email, string? PhoneNumber, string? UserPosition)
    {
        UserId = userId;
        this.CompanyId = CompanyId;
        this.OutletId = OutletId;
        this.Username = Username;
        this.Email = Email;
        this.PhoneNumber = PhoneNumber;
        this.UserPosition = UserPosition;
        IsActive = true;
        DateTimeLastLogin = null;
        DateTimeCreated = DateTime.UtcNow;
        SoftDeleted = false;
        DateTimeSoftDeleted = null;
    }

    public static Result<User> Create(Guid? OutletId,Guid CompanyId, string Username, EmailAddress Email, string? PhoneNumber, string? UserPosition)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return Result<User>.Failure(UserError.UsernameEmpty);
        }

        if (Email == null)
        {
            return Result<User>.Failure(UserError.EmailEmpty);
        }

        return Result<User>.Success(new User(Guid.CreateVersion7(), CompanyId, OutletId, Username, Email, PhoneNumber, UserPosition));
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

    public Result AssignOutletAccess(Guid outletId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (outletId == Guid.Empty)
        {
            return Result.Failure(UserOutletAccessError.OutletIdEmpty);
        }

        if (_userOutletAccesses.Any(uoa => uoa.OutletId == outletId))
        {
            return Result.Failure(UserOutletAccessError.OutletAlreadyAssigned);
        }

        var accessResult = UserOutletAccess.Create(UserId, outletId);
        if (!accessResult.IsSuccess)
        {
            return Result.Failure(accessResult.Error);
        }

        _userOutletAccesses.Add(accessResult.Data!);
        return Result.Success();
    }

    public Result RevokeOutletAccess(Guid outletId)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        var access = _userOutletAccesses.FirstOrDefault(uoa => uoa.OutletId == outletId);
        if (access == null)
        {
            return Result.Failure(UserOutletAccessError.OutletNotAssigned);
        }

        _userOutletAccesses.Remove(access);
        return Result.Success();
    }

    public Result ClearOutletAccesses()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        _userOutletAccesses.Clear();
        return Result.Success();
    }

    public bool CanAccessOutlet(Guid targetOutletId, Guid targetOutletCompanyId)
    {
        if (SoftDeleted || !IsActive)
        {
            return false;
        }

        // Must belong to the same company
        if (CompanyId != targetOutletCompanyId)
        {
            return false;
        }

        // 1. If user is explicitly assigned to a single outlet (cashier / branch staff)
        if (OutletId.HasValue)
        {
            return OutletId.Value == targetOutletId;
        }

        // 2. If user is company-wide (OutletId == null)
        // If they have specific outlet access entries (Regional Manager), they can only access those
        if (_userOutletAccesses.Count > 0)
        {
            return _userOutletAccesses.Any(uoa => uoa.OutletId == targetOutletId);
        }

        // No outlet restrictions -> Full Company Admin (access all company outlets)
        return true;
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
