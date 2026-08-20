namespace SimplePos.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<List<User>> GetUsersByOutletIdAsync(Guid outletId);
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
}
