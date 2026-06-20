namespace SimplePos.Domain.Users
{
    public class User
    {
        public Guid UserId { get; private set; }
        public Guid CompanyId { get; private set;}
        public string Username { get; private set; } = string.Empty;
        public string HashedPassword { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; } = string.Empty;
        public string? UserPosition { get; private set; } = string.Empty;
        public bool Active { get; private set; }
        public DateTime? DateTimeLastLogin { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        
        public User(Guid CompanyId, string Username, string HashedPassword, string Email, string PhoneNumber, string UserPosition)
        {
            UserId = Guid.NewGuid();
            this.CompanyId = CompanyId;
            this.Username = Username;
            this.HashedPassword = HashedPassword;
            this.Email = Email;
            this.PhoneNumber = PhoneNumber;
            this.UserPosition = UserPosition;
            Active = true;
            DateTimeLastLogin = null;
            DateTimeCreated = DateTime.UtcNow;
        }

        public void UpdateLastLogin()
        {
            DateTimeLastLogin = DateTime.UtcNow;
        }

        public void DeactivateAccount()
        {
            if (!Active)
            {
                throw new InvalidOperationException("Account is already inactive.");
            }

            Active = false;
        }

        public void ActivateAccount()
        {
            if (Active)
            {
                throw new InvalidOperationException("Account is already active.");
            }
            Active = true;
        }

        public void UpdateUserInfo(string newEmail, string newPhoneNumber, string newUserPosition)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
            {
                throw new ArgumentException("Email cannot be empty.");
            }

            Email = newEmail;
            PhoneNumber = newPhoneNumber;
            UserPosition = newUserPosition;
        }
        public void UpdatePassword(string newHashedPassword)
        {
            if (string.IsNullOrWhiteSpace(newHashedPassword))
            {
                throw new ArgumentException("New password cannot be empty.");
            }

            HashedPassword = newHashedPassword;
        }
    }
}