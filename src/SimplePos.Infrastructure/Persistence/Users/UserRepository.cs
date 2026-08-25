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
        return _appDbContext.DomainUsers.AnyAsync(u => u.Email.Value == email);
    }

    public Task<bool> ExistsByUsernameAsync(string username)
    {
        return _appDbContext.DomainUsers.AnyAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _appDbContext.DomainUsers.FirstOrDefaultAsync(u => u.Email.Value == email);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _appDbContext.DomainUsers.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _appDbContext.DomainUsers.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<List<User>> GetUsersByOutletIdAsync(Guid outletId)
    {
        return await _appDbContext.DomainUsers.Where(u => u.OutletId == outletId).ToListAsync();
    }

    public void UpdateUser(User user)
    {
        _appDbContext.DomainUsers.Update(user);
    }
}