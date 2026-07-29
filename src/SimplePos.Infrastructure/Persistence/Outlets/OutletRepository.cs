using Microsoft.EntityFrameworkCore;
using SimplePos.Domain.Outlets;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Infrastructure.Persistence.Outlets;

public class OutletRepository : IOutletRepository
{
    private readonly AppDbContext _appDbContext;

    public OutletRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task AddOutletAsync(Outlet outlet)
    {
        await _appDbContext.Outlets.AddAsync(outlet);
    }

    public async Task DeleteOutletAsync(Guid outletId)
    {
        var outlet = await GetOutletByIdAsync(outletId);
        if(outlet != null)
        {
            _appDbContext.Remove(outlet);
        }
    }

    public async Task<Outlet?> GetOutletByIdAsync(Guid outletId)
    {
        return await _appDbContext.Outlets.FirstOrDefaultAsync(o => o.OutletId.Equals(outletId));
    }

    public async Task<IEnumerable<Outlet>> GetOutletsByCompanyIdAsync(Guid companyId)
    {
        return await _appDbContext.Outlets.Where(o => o.CompanyId.Equals(companyId)).ToListAsync();
    }

    public Task UpdateOutletAsync(Outlet outlet)
    {
        throw new NotImplementedException();
    }
}
