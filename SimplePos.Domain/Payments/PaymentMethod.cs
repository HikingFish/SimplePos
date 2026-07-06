using SimplePos.Domain.Common;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Payments;
public class PaymentMethod
{
    public Guid PaymentMethodId { get; private set; }
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public bool SoftDeleted { get; private set; }

    private PaymentMethod(){}

    private PaymentMethod(Guid paymentMethodId, Guid companyId, string name)
    {
        PaymentMethodId = paymentMethodId;
        CompanyId = companyId;
        Name = name;
        IsActive = true;
        SoftDeleted = false;
    }

    public static Result<PaymentMethod> Create(Guid companyId, string name)
    {
        if (companyId == Guid.Empty)
        {
            return Result<PaymentMethod>.Failure(PaymentMethodError.CompanyIdEmpty);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<PaymentMethod>.Failure(PaymentMethodError.NameEmpty);
        }

        return Result<PaymentMethod>.Success(new PaymentMethod(Guid.CreateVersion7(), companyId, name));
    }

    public Result UpdateName(string newName)
    {
        var statusResult = EnsureNotSoftDeleted();
        if (!statusResult.IsSuccess)
        {
            return statusResult;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            return Result.Failure(PaymentMethodError.NameEmpty);
        }

        Name = newName;
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
            return Result.Failure(PaymentMethodError.AlreadyActive);
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
            return Result.Failure(PaymentMethodError.AlreadyInactive);
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
            return Result.Failure(PaymentMethodError.SoftDeleted);
        }
        return Result.Success();
    }
}
