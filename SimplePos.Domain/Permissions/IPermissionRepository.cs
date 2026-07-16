using SimplePos.Domain.Users;

namespace SimplePos.Domain.Permissions;
public interface IPermissionRepository
{
    // Task AddPermissionAsync(Permission permission);
    Task<Permission?> GetPermissionByIdAsync(Guid permissionId);
    Task<Permission?> GetPermissionByNameAsync(string name);
    // Task<bool> ExistsByNameAsync(string name);
    Task<List<Permission>> GetAllPermissionsAsync();
    // Task UpdatePermissionAsync(Permission permission);
    // Task DeletePermissionAsync(Guid permissionId);
    Task AddUserPermissionAsync(UserPermission userPermission);
    Task<List<Permission>> GetPermissionsByUserIdAsync(Guid userId);
    Task DeleteUserPermissionsByUserIdAsync(Guid userId);
    Task DeleteUserPermissionAsync(Guid userId, Guid permissionId);
    Task<bool> UserPermissionExistsAsync(Guid userId, Guid permissionId);
}
