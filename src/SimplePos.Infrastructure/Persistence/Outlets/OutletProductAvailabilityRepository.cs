using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Outlets;

namespace SimplePos.Infrastructure.Persistence.Outlets;

public class OutletProductAvailabilityRepository : IOutletProductAvailabilityRepository
{
    private readonly AppDbContext _appDbContext;

    public OutletProductAvailabilityRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<OutletProductAvailability?> GetByOutletAndProductIdAsync(Guid outletId, Guid productId)
    {
        return await _appDbContext.OutletProductAvailabilities
            .FirstOrDefaultAsync(opa => opa.OutletId == outletId && opa.ProductId == productId);
    }

    public async Task<List<OutletProductAvailability>> GetByOutletIdAsync(Guid outletId, bool? onlyUnavailable = null)
    {
        var query = _appDbContext.OutletProductAvailabilities
            .Where(opa => opa.OutletId == outletId);

        if (onlyUnavailable == true)
        {
            query = query.Where(opa => !opa.IsAvailable);
        }

        return await query.ToListAsync();
    }

    public void Add(OutletProductAvailability availability)
    {
        _appDbContext.OutletProductAvailabilities.Add(availability);
    }

    public void Update(OutletProductAvailability availability)
    {
        _appDbContext.OutletProductAvailabilities.Update(availability);
    }

    public void Delete(OutletProductAvailability availability)
    {
        _appDbContext.OutletProductAvailabilities.Remove(availability);
    }
}
