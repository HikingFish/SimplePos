namespace SimplePos.Domain.Users
{
    public class User
    {
        public Guid UserId { get; private set; }
        public Guid OutletId { get; private set;}
        public string Username { get; private set; } = string.Empty;
        public string HashedPassword { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; } = string.Empty;
        public string? UserPosition { get; private set; } = string.Empty;
        public bool IsActive { get; private set; }
        public DateTime? DateTimeLastLogin { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        
        public User(Guid OutletId, string Username, string HashedPassword, string Email, string PhoneNumber, string UserPosition)
        {
            UserId = Guid.NewGuid();
            this.OutletId = OutletId;
            this.Username = Username;
            this.HashedPassword = HashedPassword;
            this.Email = Email;
            this.PhoneNumber = PhoneNumber;
            this.UserPosition = UserPosition;
            IsActive = true;
            DateTimeLastLogin = null;
            DateTimeCreated = DateTime.UtcNow;
        }

        public void UpdateLastLogin()
        {
            DateTimeLastLogin = DateTime.UtcNow;
        }

        public void UpdateUserInfo(string newEmail, string newPhoneNumber, string newUserPosition, bool newIsActive)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
            {
                throw new ArgumentException("Email cannot be empty.");
            }

            Email = newEmail;
            PhoneNumber = newPhoneNumber;
            UserPosition = newUserPosition;
            IsActive = newIsActive;
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