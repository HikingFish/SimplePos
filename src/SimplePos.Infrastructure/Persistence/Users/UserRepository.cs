using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Users;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public void AddUser(User user)
    {
        _appDbContext.DomainUsers.Add(user);
        return;
    }

    public void DeleteUser(User user)
    {
        _appDbContext.DomainUsers.Remove(user);
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _appDbContext.DomainUsers
            .Include(u => u.UserPermissions)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetUsersByOutletIdAsync(Guid outletId)
    {
        throw new NotImplementedException();
    }

    public void UpdateUser(User user)
    {
        throw new NotImplementedException();
    }
}