namespace SimplePos.Domain.Users;

public interface IUserOutletAccessRepository
{
    void AddUserOutletAccess(UserOutletAccess userOutletAccess);
    Task<List<UserOutletAccess>> GetUserOutletAccessesByUserIdAsync(Guid userId);
    Task<UserOutletAccess?> GetUserOutletAccessAsync(Guid userId, Guid outletId);
    Task<bool> UserOutletAccessExistsAsync(Guid userId, Guid outletId);
    void DeleteUserOutletAccess(UserOutletAccess userOutletAccess);
    void DeleteUserOutletAccesses(IEnumerable<UserOutletAccess> userOutletAccesses);
}
