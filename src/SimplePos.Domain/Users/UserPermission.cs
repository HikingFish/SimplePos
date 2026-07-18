using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Permissions;

namespace SimplePos.Domain.Users;
public class UserPermission
{
    public Guid UserId { get; private set; }
    public Guid PermissionId { get; private set; }
    private UserPermission() { }
    private UserPermission(Guid userId, Guid permissionId)
    {
        UserId = userId;
        PermissionId = permissionId;
    }

    public static Result<UserPermission> Create(Guid userId, Guid permissionId)
    {
        if (userId == Guid.Empty)
        {
            return Result<UserPermission>.Failure(PermissionError.UserIdEmpty);
        }

        if (permissionId == Guid.Empty)
        {
            return Result<UserPermission>.Failure(PermissionError.PermissionIdEmpty);
        }

        return Result<UserPermission>.Success(new UserPermission(userId, permissionId));
    }
}
