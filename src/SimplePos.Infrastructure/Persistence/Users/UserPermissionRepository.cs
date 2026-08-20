using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Permissions;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Users;

public class UserPermissionRepository : IUserPermissionRepository
{
    private readonly AppDbContext _appDbContext;

    public UserPermissionRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddUserPermission(UserPermission userPermission)
    {
        _appDbContext.UserPermissions.Add(userPermission);
    }

    public void DeleteUserPermission(UserPermission userPermission)
    {
        _appDbContext.UserPermissions.Remove(userPermission);
    }

    public void DeleteUserPermissions(IEnumerable<UserPermission> userPermissions)
    {
        _appDbContext.UserPermissions.RemoveRange(userPermissions);
    }

    public async Task<List<UserPermission>> GetUserPermissionsByUserIdAsync(Guid userId)
    {
        return await _appDbContext.UserPermissions
            .Where(up => up.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserPermission?> GetUserPermissionAsync(Guid userId, Guid permissionId)
    {
        return await _appDbContext.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);
    }

    public async Task<List<Permission>> GetPermissionsByUserIdAsync(Guid userId)
    {
        return await (from up in _appDbContext.UserPermissions
                      join p in _appDbContext.Permissions on up.PermissionId equals p.PermissionId
                      where up.UserId == userId
                      select p).ToListAsync();
    }

    public async Task<bool> UserPermissionExistsAsync(Guid userId, Guid permissionId)
    {
        return await _appDbContext.UserPermissions
            .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId);
    }
}
