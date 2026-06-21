using SimplePos.Domain.Common;

namespace SimplePos.Domain.Companies
{
    public class Company
    {
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public EmailAddress Email { get; private set; }
        public string PhoneNumber { get; private set; } = string.Empty;
        public DateTime DateTimeCreated { get; private set; }
        public DateTime DateTimeLastOnline { get; private set; }
        public bool IsActive{ get; private set; }
        public bool SoftDeleted { get; private set; }

        private Company() { }

        public static Company Create(string name, string address, string phoneNumber, EmailAddress email)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException("Company name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new DomainException("Company address cannot be empty.");
            }

            if (email == null)
            {
                throw new DomainException("Company email cannot be null.");
            }

            var now = DateTime.UtcNow;

            var company = new Company
            {
                CompanyId = Guid.NewGuid(),
                Name = name,
                Address = address,
                Email = email,
                PhoneNumber = phoneNumber,
                DateTimeCreated = now,
                DateTimeLastOnline = now,
                IsActive = true,
                SoftDeleted = false
            };

            return company;
        }

        public void UpdateCompanyInfo(string newName, string newAddress, string newPhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new DomainException("Company name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(newAddress))
            {
                throw new DomainException("Company address cannot be empty.");
            }

            Name = newName;
            Address = newAddress;
            PhoneNumber = newPhoneNumber;
        }
        public void UpdateEmail(EmailAddress newEmail)
        {
            if (newEmail == null)
            {
                throw new DomainException("Company email cannot be null.");
            }

            Email = newEmail;
        }
        public void UpdateToActiveStatus()
        {
            if (IsActive)
            {
                throw new DomainException("Company is already active.");
            }

            IsActive = true;
        }
        public void UpdateToNotActiveStatus()
        {
            if (!IsActive)
            {
                throw new DomainException("Company is already inactive.");
            }

            IsActive = false;
        }
        public void UpdateLastOnline()
        {
            DateTimeLastOnline = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            if (SoftDeleted)
            {
                throw new DomainException("Company is already soft deleted.");
            }

            SoftDeleted = true;
        }
    }
}