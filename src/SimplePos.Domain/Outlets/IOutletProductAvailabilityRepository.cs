namespace SimplePos.Domain.Outlets;

public interface IOutletProductAvailabilityRepository
{
    Task<OutletProductAvailability?> GetByOutletAndProductIdAsync(Guid outletId, Guid productId);
    Task<List<OutletProductAvailability>> GetByOutletIdAsync(Guid outletId, bool? onlyUnavailable = null);
    void Add(OutletProductAvailability availability);
    void Update(OutletProductAvailability availability);
    void Delete(OutletProductAvailability availability);
}
