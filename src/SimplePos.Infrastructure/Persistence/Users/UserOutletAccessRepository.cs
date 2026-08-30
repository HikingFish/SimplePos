using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Users;

public class UserOutletAccessRepository : IUserOutletAccessRepository
{
    private readonly AppDbContext _appDbContext;

    public UserOutletAccessRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddUserOutletAccess(UserOutletAccess userOutletAccess)
    {
        _appDbContext.UserOutletAccesses.Add(userOutletAccess);
    }

    public void DeleteUserOutletAccess(UserOutletAccess userOutletAccess)
    {
        _appDbContext.UserOutletAccesses.Remove(userOutletAccess);
    }

    public void DeleteUserOutletAccesses(IEnumerable<UserOutletAccess> userOutletAccesses)
    {
        _appDbContext.UserOutletAccesses.RemoveRange(userOutletAccesses);
    }

    public async Task<List<UserOutletAccess>> GetUserOutletAccessesByUserIdAsync(Guid userId)
    {
        return await _appDbContext.UserOutletAccesses
            .Where(uoa => uoa.UserId == userId)
            .ToListAsync();
    }

    public async Task<UserOutletAccess?> GetUserOutletAccessAsync(Guid userId, Guid outletId)
    {
        return await _appDbContext.UserOutletAccesses
            .FirstOrDefaultAsync(uoa => uoa.UserId == userId && uoa.OutletId == outletId);
    }

    public async Task<bool> UserOutletAccessExistsAsync(Guid userId, Guid outletId)
    {
        return await _appDbContext.UserOutletAccesses
            .AnyAsync(uoa => uoa.UserId == userId && uoa.OutletId == outletId);
    }
}
