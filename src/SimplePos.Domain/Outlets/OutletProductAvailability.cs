using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Outlets;

public class OutletProductAvailability
{
    public Guid OutletId { get; private set; }
    public Guid ProductId { get; private set; }
    public bool IsAvailable { get; private set; }
    public DateTime? MarkedUnavailableAt { get; private set; }
    public Guid? MarkedByUserId { get; private set; }

    private OutletProductAvailability() { }

    private OutletProductAvailability(
        Guid outletId,
        Guid productId,
        bool isAvailable,
        DateTime? markedUnavailableAt,
        Guid? markedByUserId)
    {
        OutletId = outletId;
        ProductId = productId;
        IsAvailable = isAvailable;
        MarkedUnavailableAt = markedUnavailableAt;
        MarkedByUserId = markedByUserId;
    }

    public static Result<OutletProductAvailability> Create(
        Guid outletId,
        Guid productId,
        bool isAvailable = true,
        Guid? markedByUserId = null)
    {
        if (outletId == Guid.Empty)
        {
            return Result<OutletProductAvailability>.Failure(OutletProductAvailabilityError.OutletIdEmpty);
        }

        if (productId == Guid.Empty)
        {
            return Result<OutletProductAvailability>.Failure(OutletProductAvailabilityError.ProductIdEmpty);
        }

        if (!isAvailable && (markedByUserId == null || markedByUserId == Guid.Empty))
        {
            return Result<OutletProductAvailability>.Failure(OutletProductAvailabilityError.UserIdEmpty);
        }

        DateTime? markedUnavailableAt = !isAvailable ? DateTime.UtcNow : null;
        Guid? userId = !isAvailable ? markedByUserId : null;

        return Result<OutletProductAvailability>.Success(
            new OutletProductAvailability(outletId, productId, isAvailable, markedUnavailableAt, userId));
    }

    public Result MarkUnavailable(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return Result.Failure(OutletProductAvailabilityError.UserIdEmpty);
        }

        if (!IsAvailable)
        {
            return Result.Failure(OutletProductAvailabilityError.AlreadyUnavailable);
        }

        IsAvailable = false;
        MarkedUnavailableAt = DateTime.UtcNow;
        MarkedByUserId = userId;
        return Result.Success();
    }

    public Result MarkAvailable()
    {
        if (IsAvailable)
        {
            return Result.Failure(OutletProductAvailabilityError.AlreadyAvailable);
        }

        IsAvailable = true;
        MarkedUnavailableAt = null;
        MarkedByUserId = null;
        return Result.Success();
    }
}
