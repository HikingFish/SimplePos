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

    public void AddOutlet(Outlet outlet)
    {
        _appDbContext.Outlets.Add(outlet);
    }

    public void DeleteOutlet(Outlet outlet)
    {
        _appDbContext.Outlets.Remove(outlet);
    }

    public async Task<Outlet?> GetOutletByIdAsync(Guid outletId)
    {
        return await _appDbContext.Outlets.FirstOrDefaultAsync(o => o.OutletId.Equals(outletId));
    }

    public async Task<IEnumerable<Outlet>> GetOutletsByCompanyIdAsync(Guid companyId)
    {
        return await _appDbContext.Outlets.Where(o => o.CompanyId.Equals(companyId)).ToListAsync();
    }

    public void UpdateOutlet(Outlet outlet)
    {
        _appDbContext.Outlets.Update(outlet);
    }
}
