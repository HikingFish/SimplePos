using SimplePos.Domain.Permissions;

namespace SimplePos.Domain.Users;

public interface IUserPermissionRepository
{
    void AddUserPermission(UserPermission userPermission);
    Task<List<Permission>> GetPermissionsByUserIdAsync(Guid userId);
    Task<List<UserPermission>> GetUserPermissionsByUserIdAsync(Guid userId);
    Task<UserPermission?> GetUserPermissionAsync(Guid userId, Guid permissionId);
    void DeleteUserPermission(UserPermission userPermission);
    void DeleteUserPermissions(IEnumerable<UserPermission> userPermissions);
    Task<bool> UserPermissionExistsAsync(Guid userId, Guid permissionId);
}
