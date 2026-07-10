using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Companies;
public class Company : ISoftDeletable
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
    public DateTime? DateTimeSoftDeleted { get; private set; }

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
        DateTimeSoftDeleted = null;
    }

    public static Result<Company> Create(string name, Address companyAddress, string phoneNumber, EmailAddress email)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Company>.Failure(CompanyError.CompanyNameEmpty);
        }

        if (companyAddress == null)
        {
            return Result<Company>.Failure(CompanyError.CompanyAddressNull);
        }

        if (email == null)
        {
            return Result<Company>.Failure(CompanyError.CompanyEmailNull);
        }

        var now = DateTime.UtcNow;

        var company = new Company
        (
            Guid.CreateVersion7(),
            name,
            companyAddress,
            email,
            phoneNumber,
            now,
            now,
            true,
            false
        );

        return Result<Company>.Success(company);
    }

    public Result UpdateCompanyInfo(string newName, Address newAddress, string newPhoneNumber)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure(CompanyError.CompanyNameEmpty);
        }

        if (newAddress == null)
        {
            return Result.Failure(CompanyError.CompanyAddressNull);
        }

        Name = newName;
        CompanyAddress = newAddress;
        PhoneNumber = newPhoneNumber;
        return Result.Success();
    }
    public Result UpdateEmail(EmailAddress newEmail)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (newEmail == null)
        {
            return Result.Failure(CompanyError.CompanyEmailNull);
        }

        Email = newEmail;
        return Result.Success();
    }
    public Result UpdateToActiveStatus()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (IsActive)
        {
            return Result.Failure(CompanyError.AlreadyActive);
        }

        IsActive = true;
        return Result.Success();
    }
    public Result UpdateToNotActiveStatus()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (!IsActive)
        {
            return Result.Failure(CompanyError.AlreadyInactive);
        }

        IsActive = false;
        return Result.Success();
    }
    public Result UpdateLastOnline()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }
        DateTimeLastOnline = DateTime.UtcNow;
        return Result.Success();
    }

    public Result SoftDelete()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        SoftDeleted = true;
        DateTimeSoftDeleted = DateTime.UtcNow;
        IsActive = false;
        return Result.Success();
    }

    private Result EnsureNotSoftDeleted()
    {
        if (SoftDeleted)
        {
            return Result.Failure(CompanyError.SoftDeleted);
        }
        return Result.Success();
    }
}