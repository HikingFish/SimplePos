using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Permissions;

namespace SimplePos.Infrastructure.Persistence.Permissions;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _appDbContext;

    public PermissionRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public Task<List<Permission>> GetAllPermissionsAsync()
    {
        return _appDbContext.Permissions.ToListAsync();
    }

    public Task<Permission?> GetPermissionByIdAsync(Guid permissionId)
    {
        return _appDbContext.Permissions.FirstOrDefaultAsync(p => p.PermissionId == permissionId);
    }

    public Task<Permission?> GetPermissionByNameAsync(string name)
    {
        return _appDbContext.Permissions.FirstOrDefaultAsync(p => p.Name == name);
    }
}
