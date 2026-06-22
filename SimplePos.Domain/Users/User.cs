using SimplePos.Domain.Common;

namespace SimplePos.Domain.Users
{
    public class User
    {
        public Guid UserId { get; private set; }
        public Guid OutletId { get; private set;}
        public string Username { get; private set; } 
        public string HashedPassword { get; private set; } 
        public EmailAddress Email { get; private set; } 
        public string? PhoneNumber { get; private set; } 
        public string? UserPosition { get; private set; } 
        public bool IsActive { get; private set; }
        public DateTime? DateTimeLastLogin { get; private set; }
        public DateTime DateTimeCreated { get; private set; }
        public bool SoftDeleted { get; private set; }
        private User() { }
        
        private User(Guid OutletId, string Username, string HashedPassword, EmailAddress Email, string PhoneNumber, string UserPosition)
        {
            UserId = Guid.CreateVersion7();
            this.OutletId = OutletId;
            this.Username = Username;
            this.HashedPassword = HashedPassword;
            this.Email = Email;
            this.PhoneNumber = PhoneNumber;
            this.UserPosition = UserPosition;
            IsActive = true;
            DateTimeLastLogin = null;
            DateTimeCreated = DateTime.UtcNow;
            SoftDeleted = false;
        }

        public static User Create(Guid OutletId, string Username, string HashedPassword, EmailAddress Email, string PhoneNumber, string UserPosition)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                throw new DomainException("Username cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(HashedPassword))
            {
                throw new DomainException("Password cannot be empty.");
            }

            if (Email == null)
            {
                throw new DomainException("Email cannot be empty.");
            }

            return new User(OutletId, Username, HashedPassword, Email, PhoneNumber, UserPosition);
        }

        public void UpdateLastLogin()
        {
            EnsureNotSoftDeleted();

            DateTimeLastLogin = DateTime.UtcNow;
        }

        public void UpdateUserInfo(EmailAddress newEmail, string newPhoneNumber, string newUserPosition, bool newIsActive)
        {
            EnsureNotSoftDeleted();

            if (newEmail == null)
            {
                throw new DomainException("Email cannot be empty.");
            }

            Email = newEmail;
            PhoneNumber = newPhoneNumber;
            UserPosition = newUserPosition;
            IsActive = newIsActive;
        }
        public void UpdatePassword(string newHashedPassword)
        {
            EnsureNotSoftDeleted();

            if (string.IsNullOrWhiteSpace(newHashedPassword))
            {
                throw new DomainException("New password cannot be empty.");
            }

            HashedPassword = newHashedPassword;
        }

        public void SoftDelete()
        {
            if (SoftDeleted)
            {
                throw new InvalidOperationException("User is already soft deleted.");
            }

            SoftDeleted = true;
            IsActive = false;
        }

        public void EnsureNotSoftDeleted()
        {
            if (SoftDeleted)
            {
                throw new InvalidOperationException("Operation cannot be performed on a soft deleted user.");
            }
        }
    }
}