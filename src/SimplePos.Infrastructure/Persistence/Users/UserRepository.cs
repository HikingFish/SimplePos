using SimplePos.Domain.Users;

namespace SimplePos.Infrastructure.Persistence.Users;

public class UserRepository : IUserRepository
{
    public Task AddUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(Guid userId)
    {
        throw new NotImplementedException();
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

    public Task<User?> GetUserByIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetUsersByOutletIdAsync(Guid outletId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}