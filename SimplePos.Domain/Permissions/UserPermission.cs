namespace SimplePos.Domain.Permissions
{
    public class UserPermission
    {
        public Guid UserId { get; private set; }
        public Guid PermissionId { get; private set; }
        private UserPermission() { }
        private UserPermission(Guid userId, Guid permissionId)
        {
            UserId = userId;
            PermissionId = permissionId;
        }

        public static UserPermission Create(Guid userId, Guid permissionId)
        {
            return new UserPermission(userId, permissionId);
        }
    }
}