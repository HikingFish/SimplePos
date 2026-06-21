namespace SimplePos.Domain.Permissions
{
    public class UserPermission
    {
        public Guid UserId { get; private set; }
        public Guid PermissionId { get; private set; }

        public UserPermission(Guid userId, Guid permissionId)
        {
            UserId = userId;
            PermissionId = permissionId;
        }
    }
}