namespace SimplePos.Domain.Permissions
{
    public class Permission
    {
        public Guid PermissionId { get; private set; }
        public string Name { get; private set; } = string.Empty;

        public Permission(string name, string description)
        {
            PermissionId = Guid.NewGuid();
            Name = name;
        }
    }
}