using SimplePos.Domain.Permissions;
using SimplePos.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Permissions;

public class PermissionRepository : IPermissionRepository
{
    public Task AddUserPermissionAsync(UserPermission userPermission)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public Task<List<Domain.Permissions.Permission>> GetPermissionsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserPermissionExistsAsync(Guid userId, Guid permissionId)
    {
        throw new NotImplementedException();
    }
}
