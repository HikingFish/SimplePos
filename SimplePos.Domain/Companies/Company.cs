using SimplePos.Domain.Common;

namespace SimplePos.Domain.Companies
{
    public class Company
    {
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; } 
        public Address CompanyAddress { get; private set; } 
        public EmailAddress Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public DateTime DateTimeLastOnline { get; private set; }
        public bool IsActive{ get; private set; }
        public bool SoftDeleted { get; private set; }

        private Company() { }
        private Company(Guid companyId, string name, Address companyAddress, EmailAddress email, string phoneNumber, DateTime dateTimeCreated, DateTime dateTimeLastOnline, bool isActive, bool softDeleted)
        {
            CompanyId = companyId;
            Name = name;
            CompanyAddress = companyAddress;
            Email = email;
            PhoneNumber = phoneNumber;
            DateTimeCreated = dateTimeCreated;
            DateTimeLastOnline = dateTimeLastOnline;
            IsActive = isActive;
            SoftDeleted = softDeleted;
        }

        public static Company Create(string name, Address companyAddress, string phoneNumber, EmailAddress email)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Company name cannot be empty.");
            }

            if (companyAddress == null)
            {
                throw new DomainException("Company address cannot be null.");
            }

            if (email == null)
            {
                throw new DomainException("Company email cannot be null.");
            }

            var now = DateTime.UtcNow;

            var company = new Company
            {
                CompanyId = Guid.CreateVersion7(),
                Name = name,
                CompanyAddress = companyAddress,
                Email = email,
                PhoneNumber = phoneNumber,
                DateTimeCreated = now,
                DateTimeLastOnline = now,
                IsActive = true,
                SoftDeleted = false
            };

            return company;
        }

        public void UpdateCompanyInfo(string newName, Address newAddress, string newPhoneNumber)
        {
            EnsureNotSoftDeleted();

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new DomainException("Company name cannot be empty.");
            }

            if (newAddress == null)
            {
                throw new DomainException("Company address cannot be null.");
            }

            Name = newName;
            CompanyAddress = newAddress;
            PhoneNumber = newPhoneNumber;
        }
        public void UpdateEmail(EmailAddress newEmail)
        {
            EnsureNotSoftDeleted();

            if (newEmail == null)
            {
                throw new DomainException("Company email cannot be null.");
            }

            Email = newEmail;
        }
        public void UpdateToActiveStatus()
        {
            EnsureNotSoftDeleted();

            if (IsActive)
            {
                throw new DomainException("Company is already active.");
            }

            IsActive = true;
        }
        public void UpdateToNotActiveStatus()
        {
            EnsureNotSoftDeleted();

            if (!IsActive)
            {
                throw new DomainException("Company is already inactive.");
            }

            IsActive = false;
        }
        public void UpdateLastOnline()
        {
            EnsureNotSoftDeleted();
            DateTimeLastOnline = DateTime.UtcNow;
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
                throw new DomainException("Operation cannot be performed on a soft deleted company.");
            }
        }
    }
}