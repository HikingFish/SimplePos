using SimplePos.Domain.Common;

namespace SimplePos.Domain.Outlets
{
    public class Outlet
    {
        public Guid OutletId { get; private set; }
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; } 
        public Address OutletAddress { get; private set; } 
        public string PhoneNumber { get; private set; } 
        public DateTime DateTimeCreated { get; private set; }
        public DateTime? DateTimeLastOnline { get; private set; }
        public bool IsActive { get; private set; }
        public bool SoftDeleted { get; private set; }
        private Outlet() { }
        private Outlet(Guid companyId, string name, Address outletAddress, string phoneNumber, bool isActive, bool softDeleted)
        {
            OutletId = Guid.CreateVersion7();
            CompanyId = companyId;
            Name = name;
            OutletAddress = outletAddress;
            PhoneNumber = phoneNumber;
            DateTimeCreated = DateTime.UtcNow;
            DateTimeLastOnline = null;
            IsActive = isActive;
            SoftDeleted = softDeleted;
        }

        public static Outlet Create(Guid companyId, string name, Address outletAddress, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Outlet name cannot be empty.");
            }

            if (outletAddress == null)
            {
                throw new DomainException("Outlet address cannot be null.");
            }

            return new Outlet(companyId, name, outletAddress, phoneNumber, true, false);
        }

        public void UpdateOutletInfo(string newName, Address newAddress, string newPhoneNumber)
        {
            EnsureNotSoftDeleted();

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new DomainException("Outlet name cannot be empty.");
            }

            if (newAddress == null)
            {
                throw new DomainException("Outlet address cannot be null.");
            }

            Name = newName;
            OutletAddress = newAddress;
            PhoneNumber = newPhoneNumber;
        }

        public void UpdateLastOnline()
        {
            EnsureNotSoftDeleted();
            DateTimeLastOnline = DateTime.UtcNow;
        }

        public void UpdateToActiveStatus()
        {
            EnsureNotSoftDeleted();

            if (IsActive)
            {
                throw new DomainException("Outlet is already active.");
            }

            IsActive = true;
        }

        public void UpdateToNotActiveStatus()
        {
            EnsureNotSoftDeleted();

            if (!IsActive)
            {
                throw new DomainException("Outlet is already inactive.");
            }

            IsActive = false;
        }

        public void SoftDelete()
        {
            EnsureNotSoftDeleted();

            SoftDeleted = true;
            IsActive = false;
        }

        private void EnsureNotSoftDeleted()
        {
            if (SoftDeleted)
            {
                throw new DomainException("Operation cannot be performed on a soft deleted outlet.");
            }
        }
    }
}