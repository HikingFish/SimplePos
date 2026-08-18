using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Permissions;
using SimplePos.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Permissions;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _appDbContext;

    public PermissionRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddUserPermission(UserPermission userPermission)
    {
        _appDbContext.UserPermissions.Add(userPermission);
    }

    public Task DeleteUserPermissionAsync(Guid userId, Guid permissionId)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserPermissionsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Domain.Permissions.Permission>> GetAllPermissionsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Permissions.Permission?> GetPermissionByIdAsync(Guid permissionId)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Permissions.Permission?> GetPermissionByNameAsync(string name)
    {
        return _appDbContext.Permissions.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<List<Domain.Permissions.Permission>> GetPermissionsByUserIdAsync(Guid userId)
    {
        return await (from up in _appDbContext.UserPermissions
                join p in _appDbContext.Permissions on up.PermissionId equals p.PermissionId
                where up.UserId == userId
                select p).ToListAsync();
    }

    public Task<bool> UserPermissionExistsAsync(Guid userId, Guid permissionId)
    {
        throw new NotImplementedException();
    }
}
