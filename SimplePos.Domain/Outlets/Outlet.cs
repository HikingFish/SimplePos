namespace SimplePos.Domain.Outlets
{
    public class Outlet
    {
        public Guid OutletId { get; private set; }
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public string PhoneNumber { get; private set; } = string.Empty;
        public DateTime DateTimeCreated { get; private set; }
        public DateTime? DateTimeLastOnline { get; private set; }

        public Outlet(Guid companyId, string name, string address, string phoneNumber)
        {
            OutletId = Guid.NewGuid();
            CompanyId = companyId;
            Name = name;
            Address = address;
            PhoneNumber = phoneNumber;
            DateTimeCreated = DateTime.UtcNow;
            DateTimeLastOnline = null;
        }

        public void UpdateOutletInfo(string newName, string newAddress, string newPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Outlet name cannot be empty.");
            }

            Name = newName;
            Address = newAddress;
            PhoneNumber = newPhoneNumber;
        }

        public void UpdateLastOnline()
        {
            DateTimeLastOnline = DateTime.UtcNow;
        }
    }
}