using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Permissions;
public class Permission
{
    public Guid PermissionId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    private Permission() { }

    private Permission(Guid permissionId, string name)
    {
        PermissionId = permissionId;
        Name = name;
    }

    public static Result<Permission> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Permission>.Failure(PermissionError.PermissionNameEmpty);
        }

        return Result<Permission>.Success(new Permission(Guid.CreateVersion7(), name));
    }
}
