using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Outlets;
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
    private Outlet(Guid outletId, Guid companyId, string name, Address outletAddress, string phoneNumber, bool isActive, bool softDeleted)
    {
        OutletId = outletId;
        CompanyId = companyId;
        Name = name;
        OutletAddress = outletAddress;
        PhoneNumber = phoneNumber;
        DateTimeCreated = DateTime.UtcNow;
        DateTimeLastOnline = null;
        IsActive = isActive;
        SoftDeleted = softDeleted;
    }

    public static Result<Outlet> Create(Guid companyId, string name, Address outletAddress, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Outlet>.Failure(OutletError.OutletNameEmpty);
        }

        if (outletAddress == null)
        {
            return Result<Outlet>.Failure(OutletError.OutletAddressNull);
        }

        return Result<Outlet>.Success(new Outlet(Guid.CreateVersion7(), companyId, name, outletAddress, phoneNumber, true, false));
    }

    public Result UpdateOutletInfo(string newName, Address newAddress, string newPhoneNumber)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure(OutletError.OutletNameEmpty);
        }

        if (newAddress == null)
        {
            return Result.Failure(OutletError.OutletAddressNull);
        }

        Name = newName;
        OutletAddress = newAddress;
        PhoneNumber = newPhoneNumber;
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

    public Result UpdateToActiveStatus()
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (IsActive)
        {
            return Result.Failure(OutletError.AlreadyActive);
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
            return Result.Failure(OutletError.AlreadyInactive);
        }

        IsActive = false;
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
        IsActive = false;
        return Result.Success();
    }

    private Result EnsureNotSoftDeleted()
    {
        if (SoftDeleted)
        {
            return Result.Failure(OutletError.SoftDeleted);
        }
        return Result.Success();
    }
}