using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Users;
public class User
{
    public Guid UserId { get; private set; }
    public Guid OutletId { get; private set;}
    public string Username { get; private set; } 
    public string HashedPassword { get; private set; } 
    public EmailAddress Email { get; private set; } 
    public string? PhoneNumber { get; private set; } 
    public string? UserPosition { get; private set; } 
    public bool IsActive { get; private set; }
    public DateTime? DateTimeLastLogin { get; private set; }
    public DateTime DateTimeCreated { get; private set; }
    public bool SoftDeleted { get; private set; }
    private User() { }
    
    private User(Guid OutletId, string Username, string HashedPassword, EmailAddress Email, string PhoneNumber, string UserPosition)
    {
        UserId = Guid.CreateVersion7();
        this.OutletId = OutletId;
        this.Username = Username;
        this.HashedPassword = HashedPassword;
        this.Email = Email;
        this.PhoneNumber = PhoneNumber;
        this.UserPosition = UserPosition;
        IsActive = true;
        DateTimeLastLogin = null;
        DateTimeCreated = DateTime.UtcNow;
        SoftDeleted = false;
    }

    public static Result<User> Create(Guid OutletId, string Username, string HashedPassword, EmailAddress Email, string PhoneNumber, string UserPosition)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            return Result<User>.Failure(UserError.UsernameEmpty);
        }

        if (string.IsNullOrWhiteSpace(HashedPassword))
        {
            return Result<User>.Failure(UserError.PasswordEmpty);
        }

        if (Email == null)
        {
            return Result<User>.Failure(UserError.EmailEmpty);
        }

        return Result<User>.Success(new User(OutletId, Username, HashedPassword, Email, PhoneNumber, UserPosition));
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

        HashedPassword = newHashedPassword;
        return Result.Success();
    }

    public Result SoftDelete()
    {
        if (SoftDeleted)
        {
            return Result.Failure(UserError.AlreadySoftDeleted);
        }

        SoftDeleted = true;
        IsActive = false;
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
