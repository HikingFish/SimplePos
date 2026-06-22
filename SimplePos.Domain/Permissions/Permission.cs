namespace SimplePos.Domain.Permissions
{
    public class Permission
    {
        public Guid PermissionId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        private Permission() { }

        private Permission(string name)
        {
            PermissionId = Guid.CreateVersion7();
            Name = name;
        }

        public static Permission Create(string name)
        {
            return new Permission(name);
        }
    }
}