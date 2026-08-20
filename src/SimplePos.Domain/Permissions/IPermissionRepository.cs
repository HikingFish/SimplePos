namespace SimplePos.Domain.Permissions;

public interface IPermissionRepository
{
    // void AddPermission(Permission permission);
    Task<Permission?> GetPermissionByIdAsync(Guid permissionId);
    Task<Permission?> GetPermissionByNameAsync(string name);
    // Task<bool> ExistsByNameAsync(string name);
    Task<List<Permission>> GetAllPermissionsAsync();
    // void UpdatePermission(Permission permission);
    // void DeletePermission(Permission permission);
}
